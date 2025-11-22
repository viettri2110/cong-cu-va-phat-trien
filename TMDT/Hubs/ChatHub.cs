using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using TMDT.Models;

namespace TMDT.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatContext db = new ChatContext();

        // Gửi tin nhắn
        public async Task SendMessage(int idRoom, int idSender, string message)
        {
            try
            {
                // Lưu tin nhắn vào database
                var chatMessage = new ChatMessage
                {
                    idRoom = idRoom,
                    idSender = idSender,
                    messageContent = message,
                    dateSent = DateTime.Now,
                    isRead = false,
                    messageType = "text"
                };

                db.ChatMessages.Add(chatMessage);
                
                // Cập nhật lastMessageDate của room
                var room = db.ChatRooms.Find(idRoom);
                if (room != null)
                {
                    room.lastMessageDate = DateTime.Now;
                }
                
                await db.SaveChangesAsync();

                // Broadcast tin nhắn đến tất cả clients trong room
                await Clients.Group("Room_" + idRoom).broadcastMessage(new
                {
                    idMessage = chatMessage.idMessage,
                    idSender = idSender,
                    messageContent = message,
                    dateSent = chatMessage.dateSent.ToString("dd/MM/yyyy HH:mm"),
                    isRead = false
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.onError("Lỗi gửi tin nhắn: " + ex.Message);
            }
        }

        // Join vào room
        public async Task JoinRoom(int idRoom)
        {
            await Groups.Add(Context.ConnectionId, "Room_" + idRoom);
        }

        // Leave room
        public async Task LeaveRoom(int idRoom)
        {
            await Groups.Remove(Context.ConnectionId, "Room_" + idRoom);
        }

        // Đánh dấu đã đọc
        public async Task MarkAsRead(int idRoom, int idUser)
        {
            var messages = db.ChatMessages.Where(m => m.idRoom == idRoom && m.idSender != idUser && !m.isRead).ToList();
            foreach (var msg in messages)
            {
                msg.isRead = true;
            }
            await db.SaveChangesAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
