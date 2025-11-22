## 📨 HƯỚNG DẪN CÀI ĐẶT HỆ THỐNG TIN NHẮN & OFFER

### 1. CHẠY SQL SCRIPT
Mở SQL Server Management Studio và chạy file:
```
SQL_CreateMessageOffersTable.sql
```
Script này sẽ tạo bảng `MessageOffers` trong database.

### 2. BUILD LẠI PROJECT
- Mở Visual Studio
- Build lại solution để compile các Model mới
- Nếu có lỗi về DbContext, hãy Build lại

### 3. THÊM LINK VÀO MENU NGƯỜI BÁN
Thêm link "Tin nhắn" vào menu dashboard của người bán.

Tìm file menu (thường là `_Dashboard_1.cshtml` hoặc `_MenuSeller.cshtml`) và thêm:

```html
<li class="nav-item">
    <a href="@Url.Action("DanhSachTinNhan", "Seller")" class="nav-link">
        <i class="fas fa-comments"></i>
        <p>
            Tin nhắn & Offer
            <span class="badge badge-warning right" id="unreadMessageBadge"></span>
        </p>
    </a>
</li>
```

### 4. THÊM NOTIFICATION SCRIPT (Tùy chọn)
Thêm vào layout để hiển thị số tin nhắn chưa đọc:

```javascript
<script>
// Auto update unread message count
setInterval(function() {
    $.get('@Url.Action("GetUnreadMessageCount", "Seller")', function(data) {
        if (data.count > 0) {
            $('#unreadMessageBadge').text(data.count).show();
        } else {
            $('#unreadMessageBadge').hide();
        }
    });
}, 30000); // Mỗi 30 giây
</script>
```

### 5. TẠO FORM GỬI OFFER CHO NGƯỜI MUA
Thêm nút "Đưa ra offer" trong trang chi tiết sản phẩm (`ChiTietSanPham.cshtml`):

```html
<button type="button" class="btn btn-warning" data-toggle="modal" data-target="#offerModal">
    <i class="fas fa-hand-holding-usd"></i> Đưa ra giá Offer
</button>

<!-- Modal Offer -->
<div class="modal fade" id="offerModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Đưa ra giá Offer</h5>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            <form action="@Url.Action("GuiOffer", "Buyer")" method="post">
                @Html.AntiForgeryToken()
                <div class="modal-body">
                    <input type="hidden" name="idProduct" value="@Model.idProduct" />
                    <input type="hidden" name="idSeller" value="@Model.idAccount" />
                    
                    <div class="form-group">
                        <label>Giá sản phẩm hiện tại:</label>
                        <h4 style="color: #ffba00;">@string.Format("{0:N0}", Model.priceProduct) ₫</h4>
                    </div>
                    
                    <div class="form-group">
                        <label>Giá bạn muốn mua:</label>
                        <input type="number" name="offerPrice" class="form-control" 
                               placeholder="Nhập giá..." required min="1000" 
                               step="1000" />
                    </div>
                    
                    <div class="form-group">
                        <label>Tin nhắn cho người bán:</label>
                        <textarea name="messageContent" class="form-control" rows="4" 
                                  placeholder="Nhập tin nhắn..." required></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
                    <button type="submit" class="btn btn-warning">Gửi Offer</button>
                </div>
            </form>
        </div>
    </div>
</div>
```

### 6. TẠO ACTION TRONG BUYERCONTROLLER
Thêm vào `BuyerController.cs`:

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult GuiOffer(int idProduct, int idSeller, decimal offerPrice, string messageContent)
{
    if (Session["idAccount"] == null)
        return RedirectToAction("DangNhap", "Login");
    
    int buyerId = Int32.Parse(Session["idAccount"].ToString());
    
    if (buyerId == idSeller)
    {
        TempData["Error"] = "Bạn không thể gửi offer cho chính mình!";
        return RedirectToAction("ChiTietSanPham", "Home", new { id = idProduct });
    }
    
    MessageOffer offer = new MessageOffer
    {
        idProduct = idProduct,
        idBuyer = buyerId,
        idSeller = idSeller,
        messageContent = messageContent,
        offerPrice = offerPrice,
        senderType = "buyer",
        dateMessage = DateTime.Now,
        isRead = false,
        status = "pending"
    };
    
    db.MessageOffers.Add(offer);
    db.SaveChanges();
    
    // Gửi email thông báo cho người bán
    try
    {
        var sellerEmail = (from login in db.Logins
                          where login.idAccount == idSeller
                          select login.Email).FirstOrDefault();
        
        if (!string.IsNullOrEmpty(sellerEmail))
        {
            string emailContent = $@"
                <h3>Bạn có offer mới!</h3>
                <p>Có người quan tâm đến sản phẩm của bạn.</p>
                <p><strong>Giá offer:</strong> {offerPrice:N0} ₫</p>
                <p>Vui lòng đăng nhập để xem chi tiết và phản hồi.</p>
            ";
            new MailHelper().SendMail(sellerEmail, "[Chợ Đồ Cũ] Bạn có offer mới!", emailContent);
        }
    }
    catch { }
    
    TempData["Success"] = "Đã gửi offer thành công! Vui lòng chờ người bán phản hồi.";
    return RedirectToAction("ChiTietSanPham", "Home", new { id = idProduct });
}
```

### 7. TÍNH NĂNG BỔ SUNG
- ✅ Người bán xem danh sách tin nhắn/offer
- ✅ Người bán trả lời, chấp nhận hoặc từ chối offer
- ✅ Hiển thị số tin nhắn chưa đọc
- ✅ Gửi email thông báo tự động
- ✅ Lọc tin nhắn theo trạng thái
- ✅ Giao diện chat đẹp mắt

### 8. KIỂM TRA
1. Đăng nhập tài khoản người mua
2. Vào trang chi tiết sản phẩm
3. Nhấn "Đưa ra Offer"
4. Đăng nhập tài khoản người bán
5. Vào "Tin nhắn & Offer"
6. Xem và trả lời tin nhắn

### LƯU Ý QUAN TRỌNG:
- Phải chạy SQL script trước khi sử dụng
- Build lại project sau khi thêm Model mới
- Đảm bảo cấu hình email trong Web.config để gửi thông báo

Chúc bạn triển khai thành công! 🎉
