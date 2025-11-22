using System;
using System.Linq;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatContext chatDb = new ChatContext();
        private readonly ChoDoCuEntities db = new ChoDoCuEntities();

        // Mở chat từ trang chi tiết sản phẩm
        public ActionResult StartChat(int idProduct, int idSeller)
        {
            if (Session["idAccount"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int idBuyer = int.Parse(Session["idAccount"].ToString());

            // Kiểm tra xem đã có phòng chat chưa
            var existingRoom = chatDb.ChatRooms
                .FirstOrDefault(r => r.idProduct == idProduct && r.idBuyer == idBuyer && r.idSeller == idSeller);

            int roomId;
            if (existingRoom == null)
            {
                // Tạo phòng chat mới
                var newRoom = new ChatRoom
                {
                    idProduct = idProduct,
                    idBuyer = idBuyer,
                    idSeller = idSeller,
                    dateCreated = DateTime.Now,
                    isActive = true
                };
                chatDb.ChatRooms.Add(newRoom);
                chatDb.SaveChanges();
                roomId = newRoom.idRoom;
            }
            else
            {
                roomId = existingRoom.idRoom;
            }

            return RedirectToAction("ChatRoom", new { idRoom = roomId });
        }

        // Phòng chat
        public ActionResult ChatRoom(int idRoom)
        {
            if (Session["idAccount"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int currentUserId = int.Parse(Session["idAccount"].ToString());

            var room = chatDb.ChatRooms.Find(idRoom);
            if (room == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra quyền truy cập
            if (room.idBuyer != currentUserId && room.idSeller != currentUserId)
            {
                return new HttpUnauthorizedResult();
            }

            // Lấy thông tin sản phẩm và người dùng
            var product = db.Products.Find(room.idProduct);
            int otherUserId = room.idBuyer == currentUserId ? room.idSeller : room.idBuyer;
            var otherUser = db.Logins.Find(otherUserId);
            var otherUserInfo = db.infoAccounts.FirstOrDefault(i => i.idAccount == otherUserId);

            ViewBag.RoomId = idRoom;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.ProductName = product?.nameProduct;
            ViewBag.ProductImage = product?.imageProduct_1;
            ViewBag.ProductPrice = product?.priceProduct;
            ViewBag.OtherUserName = otherUserInfo?.Fullname ?? otherUser?.Email;
            ViewBag.IsSeller = (room.idSeller == currentUserId);

            // Lấy lịch sử tin nhắn
            var messages = chatDb.ChatMessages
                .Where(m => m.idRoom == idRoom)
                .OrderBy(m => m.dateSent)
                .Select(m => new ChatMessageViewModel
                {
                    idMessage = m.idMessage,
                    idSender = m.idSender,
                    messageContent = m.messageContent,
                    dateSent = m.dateSent,
                    isRead = m.isRead,
                    isMyMessage = m.idSender == currentUserId
                })
                .ToList();

            // Đánh dấu tin nhắn là đã đọc
            var unreadMessages = chatDb.ChatMessages
                .Where(m => m.idRoom == idRoom && m.idSender != currentUserId && !m.isRead)
                .ToList();
            foreach (var msg in unreadMessages)
            {
                msg.isRead = true;
            }
            if (unreadMessages.Any())
            {
                chatDb.SaveChanges();
            }

            return View(messages);
        }

        // Xem chi tiết chat (Dành cho Admin)
        public ActionResult XemChiTietChat(int idRoom)
        {
            if (Session["idAccount"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            // Chỉ admin mới được truy cập
            int? idRole = Session["idRole"] as int?;
            if (idRole != 4 && idRole != 5)
            {
                return RedirectToAction("ChatRoom", new { idRoom = idRoom });
            }

            int currentUserId = int.Parse(Session["idAccount"].ToString());

            var room = chatDb.ChatRooms.Find(idRoom);
            if (room == null)
            {
                return HttpNotFound();
            }

            // Admin có thể xem tất cả phòng chat
            var product = db.Products.Find(room.idProduct);
            var buyer = db.Logins.Find(room.idBuyer);
            var buyerInfo = db.infoAccounts.FirstOrDefault(i => i.idAccount == room.idBuyer);
            var seller = db.Logins.Find(room.idSeller);
            var sellerInfo = db.infoAccounts.FirstOrDefault(i => i.idAccount == room.idSeller);

            ViewBag.RoomId = idRoom;
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.ProductName = product?.nameProduct;
            ViewBag.ProductImage = product?.imageProduct_1;
            ViewBag.ProductPrice = product?.priceProduct;
            ViewBag.BuyerName = buyerInfo?.Fullname ?? buyer?.Email;
            ViewBag.SellerName = sellerInfo?.Fullname ?? seller?.Email;
            ViewBag.IsAdmin = true;

            // Lấy lịch sử tin nhắn
            var messages = chatDb.ChatMessages
                .Where(m => m.idRoom == idRoom)
                .OrderBy(m => m.dateSent)
                .Select(m => new ChatMessageViewModel
                {
                    idMessage = m.idMessage,
                    idSender = m.idSender,
                    messageContent = m.messageContent,
                    dateSent = m.dateSent,
                    isRead = m.isRead,
                    isMyMessage = false // Admin view, không cần phân biệt
                })
                .ToList();

            // Đánh dấu tất cả tin nhắn là đã đọc
            var unreadMessages = chatDb.ChatMessages
                .Where(m => m.idRoom == idRoom && !m.isRead)
                .ToList();
            foreach (var msg in unreadMessages)
            {
                msg.isRead = true;
            }
            if (unreadMessages.Any())
            {
                chatDb.SaveChanges();
            }

            return View(messages);
        }

        // Quản lý tất cả chat (Dành cho Admin)
        public ActionResult QuanLyChat()
        {
            if (Session["idAccount"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            // Chỉ admin mới được truy cập
            int? idRole = Session["idRole"] as int?;
            if (idRole != 4 && idRole != 5)
            {
                return RedirectToAction("MyChats");
            }

            // Lấy tất cả phòng chat trong hệ thống
            var rooms = chatDb.ChatRooms.ToList();

            var chatList = rooms.Select(room =>
            {
                var product = db.Products.FirstOrDefault(p => p.idProduct == room.idProduct);
                var buyer = db.Logins.FirstOrDefault(b => b.idAccount == room.idBuyer);
                var buyerInfo = db.infoAccounts.FirstOrDefault(i => i.idAccount == room.idBuyer);
                var seller = db.Logins.FirstOrDefault(s => s.idAccount == room.idSeller);
                var sellerInfo = db.infoAccounts.FirstOrDefault(i => i.idAccount == room.idSeller);
                
                var lastMsg = chatDb.ChatMessages
                    .Where(m => m.idRoom == room.idRoom)
                    .OrderByDescending(m => m.dateSent)
                    .FirstOrDefault();
                
                var unreadCount = chatDb.ChatMessages.Count(m => m.idRoom == room.idRoom && !m.isRead);

                return new ChatRoomViewModel
                {
                    idRoom = room.idRoom,
                    idProduct = product?.idProduct ?? 0,
                    productName = product?.nameProduct ?? "Sản phẩm không tồn tại",
                    productImage = product?.imageProduct_1,
                    productPrice = product?.priceProduct ?? 0,
                    otherUserId = buyer?.idAccount ?? 0,
                    otherUserName = $"Buyer: {buyerInfo?.Fullname ?? buyer?.Email ?? "Unknown"} | Seller: {sellerInfo?.Fullname ?? seller?.Email ?? "Unknown"}",
                    otherUserEmail = buyer?.Email ?? "",
                    lastMessage = lastMsg?.messageContent,
                    lastMessageDate = lastMsg?.dateSent,
                    unreadCount = unreadCount
                };
            }).OrderByDescending(c => c.lastMessageDate).ToList();

            return View(chatList);
        }

        // Danh sách chat của người mua
        public ActionResult MyChats()
        {
            if (Session["idAccount"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int userId = int.Parse(Session["idAccount"].ToString());

            // Bước 1: Lấy rooms từ chatDb
            var rooms = chatDb.ChatRooms.Where(r => r.idBuyer == userId).ToList();

            // Bước 2: Tạo list ViewModel bằng cách query từng phần
            var chatList = rooms.Select(room =>
            {
                // Lấy thông tin product từ db
                var product = db.Products.FirstOrDefault(p => p.idProduct == room.idProduct);
                
                // Lấy thông tin seller từ db
                var seller = db.Logins.FirstOrDefault(s => s.idAccount == room.idSeller);
                var sellerInfo = db.infoAccounts.FirstOrDefault(i => i.idAccount == room.idSeller);
                
                // Lấy tin nhắn cuối cùng từ chatDb
                var lastMsg = chatDb.ChatMessages
                    .Where(m => m.idRoom == room.idRoom)
                    .OrderByDescending(m => m.dateSent)
                    .FirstOrDefault();
                
                // Đếm số tin nhắn chưa đọc
                var unreadCount = chatDb.ChatMessages
                    .Count(m => m.idRoom == room.idRoom && m.idSender != userId && !m.isRead);

                return new ChatRoomViewModel
                {
                    idRoom = room.idRoom,
                    idProduct = product?.idProduct ?? 0,
                    productName = product?.nameProduct ?? "Sản phẩm không tồn tại",
                    productImage = product?.imageProduct_1,
                    productPrice = product?.priceProduct ?? 0,
                    otherUserId = seller?.idAccount ?? 0,
                    otherUserName = sellerInfo?.Fullname ?? seller?.Email ?? "Unknown",
                    otherUserEmail = seller?.Email ?? "",
                    lastMessage = lastMsg?.messageContent,
                    lastMessageDate = lastMsg?.dateSent,
                    unreadCount = unreadCount
                };
            }).OrderByDescending(c => c.lastMessageDate).ToList();

            return View(chatList);
        }

        // ===== CHATBOT / ADMIN CHAT =====
        
        /// <summary>
        /// Quản lý ChatBot (Dành cho Admin)
        /// </summary>
        public ActionResult QuanLyChatBot()
        {
            if (Session["idAccount"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            // Chỉ admin mới được truy cập
            int? idRole = Session["idRole"] as int?;
            if (idRole != 4 && idRole != 5)
            {
                return HttpNotFound();
            }

            // Lấy danh sách người dùng đã chat với chatbot
            var users = chatDb.ChatBotMessages
                .GroupBy(m => m.idUser)
                .Select(g => g.OrderByDescending(m => m.dateSent).FirstOrDefault())
                .ToList();

            var viewModel = users.Select(msg =>
            {
                var user = db.Logins.FirstOrDefault(l => l.idAccount == msg.idUser);
                var userInfo = db.infoAccounts.FirstOrDefault(i => i.idAccount == msg.idUser);
                var unreadCount = chatDb.ChatBotMessages
                    .Count(m => m.idUser == msg.idUser && !m.isFromAdmin && !m.isRead);

                return new ChatBotUserViewModel
                {
                    idUser = msg.idUser,
                    userName = userInfo?.Fullname ?? user?.Email ?? "Unknown",
                    userEmail = user?.Email ?? "",
                    lastMessage = msg.messageContent,
                    lastMessageDate = msg.dateSent,
                    unreadCount = unreadCount
                };
            }).OrderByDescending(u => u.lastMessageDate).ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Chi tiết chat với user qua chatbot (Dành cho Admin)
        /// </summary>
        public ActionResult ChatBotDetail(int idUser)
        {
            if (Session["idAccount"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            // Chỉ admin mới được truy cập
            int? idRole = Session["idRole"] as int?;
            if (idRole != 4 && idRole != 5)
            {
                return HttpNotFound();
            }

            int adminId = int.Parse(Session["idAccount"].ToString());

            // Lấy thông tin user
            var user = db.Logins.Find(idUser);
            var userInfo = db.infoAccounts.FirstOrDefault(i => i.idAccount == idUser);
            
            ViewBag.UserId = idUser;
            ViewBag.AdminId = adminId;
            ViewBag.UserName = userInfo?.Fullname ?? user?.Email ?? "Unknown";
            ViewBag.UserEmail = user?.Email;

            // Lấy lịch sử chat
            var messages = chatDb.ChatBotMessages
                .Where(m => m.idUser == idUser)
                .OrderBy(m => m.dateSent)
                .ToList();

            // Đánh dấu tin nhắn là đã đọc
            var unreadMessages = messages.Where(m => !m.isFromAdmin && !m.isRead).ToList();
            foreach (var msg in unreadMessages)
            {
                msg.isRead = true;
            }
            if (unreadMessages.Any())
            {
                chatDb.SaveChanges();
            }

            return View(messages);
        }

        /// <summary>
        /// Gửi tin nhắn từ admin đến user qua chatbot
        /// </summary>
        [HttpPost]
        public JsonResult SendAdminMessage(int idUser, string message)
        {
            if (Session["idAccount"] == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            int adminId = int.Parse(Session["idAccount"].ToString());

            var newMessage = new ChatBotMessage
            {
                idUser = idUser,
                idAdmin = adminId,
                messageContent = message,
                dateSent = DateTime.Now,
                isRead = false,
                isFromAdmin = true
            };

            chatDb.ChatBotMessages.Add(newMessage);
            chatDb.SaveChanges();

            return Json(new { success = true, messageId = newMessage.idChatBot });
        }

        /// <summary>
        /// Test ChatBot - Gửi tin nhắn từ user
        /// </summary>
        public ActionResult TestChatBotMessage()
        {
            if (Session["idAccount"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int userId = int.Parse(Session["idAccount"].ToString());
            ViewBag.UserId = userId;

            // Lấy lịch sử chat của user với chatbot
            var messages = chatDb.ChatBotMessages
                .Where(m => m.idUser == userId)
                .OrderBy(m => m.dateSent)
                .ToList();

            return View(messages);
        }

        /// <summary>
        /// Gửi tin nhắn từ user đến chatbot (YÊU CẦU ĐĂNG NHẬP)
        /// </summary>
        [HttpPost]
        public JsonResult SendChatBotMessage(string message)
        {
            if (Session["idAccount"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để sử dụng chatbot", requireLogin = true });
            }

            int userId = int.Parse(Session["idAccount"].ToString());

            // Kiểm tra xem tin nhắn có phải khiếu nại không
            bool isComplaint = DetectComplaint(message);

            var newMessage = new ChatBotMessage
            {
                idUser = userId,
                messageContent = message,
                dateSent = DateTime.Now,
                isRead = false,
                isFromAdmin = false
            };

            chatDb.ChatBotMessages.Add(newMessage);
            chatDb.SaveChanges();

            // Kiểm tra xem đây có phải tin nhắn đầu tiên của user không
            int messageCount = chatDb.ChatBotMessages.Count(m => m.idUser == userId && !m.isFromAdmin);
            bool isFirstMessage = messageCount == 1; // Vừa mới thêm 1 tin nhắn ở trên

            string responseMessage = "";
            
            // Chỉ tự động trả lời tin nhắn đầu tiên
            if (isFirstMessage)
            {
                if (isComplaint)
                {
                    responseMessage = "Chúng tôi đã ghi nhận khiếu nại của bạn. Admin sẽ liên hệ trong thời gian sớm nhất.";
                }
                else
                {
                    responseMessage = GetAutomatedResponse(message);
                }

                // Gửi phản hồi tự động từ bot
                var botResponse = new ChatBotMessage
                {
                    idUser = userId,
                    messageContent = responseMessage,
                    dateSent = DateTime.Now.AddSeconds(1),
                    isRead = false,
                    isFromAdmin = true,
                    idAdmin = null // null = automated bot response
                };

                chatDb.ChatBotMessages.Add(botResponse);
                chatDb.SaveChanges();
            }

            return Json(new
            {
                success = true,
                messageId = newMessage.idChatBot,
                isComplaint = isComplaint,
                botResponse = isFirstMessage ? responseMessage : null,
                isFirstMessage = isFirstMessage
            });
        }

        /// <summary>
        /// Lấy lịch sử chat của user hiện tại với admin (YÊU CẦU ĐĂNG NHẬP)
        /// </summary>
        [HttpGet]
        public JsonResult GetMyChatBotMessages()
        {
            if (Session["idAccount"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" }, JsonRequestBehavior.AllowGet);
            }

            int userId = int.Parse(Session["idAccount"].ToString());

            var messages = chatDb.ChatBotMessages
                .Where(m => m.idUser == userId)
                .OrderBy(m => m.dateSent)
                .Select(m => new
                {
                    id = m.idChatBot,
                    content = m.messageContent,
                    isFromAdmin = m.isFromAdmin,
                    dateSent = m.dateSent,
                    isRead = m.isRead
                })
                .ToList();

            return Json(new
            {
                success = true,
                messages = messages
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Phát hiện khiếu nại/phàn nàn
        /// </summary>
        private bool DetectComplaint(string message)
        {
            string lowerMessage = message.ToLower();
            string[] complaintKeywords = {
                "khiếu nại", "phàn nàn", "tố cáo", "báo cáo",
                "lừa đảo", "gian lận", "không nhận được hàng",
                "sản phẩm lỗi", "kém chất lượng", "không như mô tả",
                "yêu cầu hoàn tiền", "hoàn tiền", "đền bù",
                "tệ", "tệ quá", "quá tệ", "dở", "kém", "thất vọng",
                "không hài lòng", "không ổn", "tồi", "không tốt",
                "rất tệ", "quá dở", "chán", "thất bại",
                "lẽ ra", "có thể", "giúp", "hỗ trợ", "sao",
                "tại sao", "vì sao", "làm sao", "thế nào"
            };

            return complaintKeywords.Any(keyword => lowerMessage.Contains(keyword));
        }

        /// <summary>
        /// Phản hồi tự động từ chatbot
        /// </summary>
        private string GetAutomatedResponse(string message)
        {
            string lowerMessage = message.ToLower();

            if (lowerMessage.Contains("chào") || lowerMessage.Contains("hello") || lowerMessage.Contains("hi"))
            {
                return "Xin chào! Tôi là trợ lý ảo của ChoDoCu. Bạn cần hỗ trợ gì?";
            }
            else if (lowerMessage.Contains("giá") || lowerMessage.Contains("bao nhiêu"))
            {
                return "Bạn vui lòng xem giá trên trang sản phẩm nhé. Nếu cần hỗ trợ thêm, hãy chat trực tiếp với người bán.";
            }
            else if (lowerMessage.Contains("ship") || lowerMessage.Contains("giao hàng") || lowerMessage.Contains("vận chuyển"))
            {
                return "Thông tin về phí vận chuyển và thời gian giao hàng sẽ hiển thị khi bạn đặt hàng. Mỗi người bán có chính sách khác nhau.";
            }
            else if (lowerMessage.Contains("thanh toán") || lowerMessage.Contains("payment"))
            {
                return "ChoDoCu hỗ trợ thanh toán qua PayPal và COD. Bạn có thể chọn phương thức thanh toán khi đặt hàng.";
            }
            else
            {
                return "Cảm ơn bạn đã liên hệ. Nếu cần hỗ trợ thêm, vui lòng chat với người bán hoặc liên hệ admin.";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                chatDb.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
