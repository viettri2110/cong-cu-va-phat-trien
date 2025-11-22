# 🤖 HƯỚNG DẪN SỬ DỤNG CHATBOT - NGƯỜI DÙNG

## 📱 TÍNH NĂNG

Widget ChatBot xuất hiện ở **góc phải dưới màn hình** với 2 chế độ:

### 1. **Chế độ Tự động (Auto Reply)** 🤖
- Trả lời tự động các câu hỏi thường gặp
- Hơn 100+ mẫu câu trả lời được lập trình sẵn
- Phản hồi ngay lập tức 24/7

### 2. **Chế độ Chat với Admin** 👨‍💼
- Chat trực tiếp với Admin/Nhân viên
- Tin nhắn được lưu vào database
- Nhận thông báo khi Admin trả lời

---

## 🎯 CÁCH SỬ DỤNG

### Bước 1: Mở ChatBot

1. Tìm **icon chat màu vàng** ở góc phải dưới màn hình
2. Click vào icon để mở cửa sổ chat
3. Bạn sẽ thấy lời chào từ chatbot

### Bước 2: Chat Tự động

**Mặc định, chatbot ở chế độ trả lời tự động:**

- Gõ câu hỏi như:
  - "Giờ làm việc"
  - "Liên hệ"
  - "Cách mua hàng"
  - "Đăng bán sản phẩm"
  - "Thanh toán"
  
- Chatbot sẽ trả lời ngay lập tức

**Câu hỏi về Offer:**
- "Đưa ra offer"
- "Trả giá"
- "Chào giá"
- "Người bán không trả lời"
- "Hủy offer"

### Bước 3: Chuyển sang Chat với Admin

Nếu câu trả lời tự động không đủ, bạn có thể chat trực tiếp với Admin:

1. Click vào **icon 👨‍💼** ở góc trên phải cửa sổ chat
2. Trạng thái sẽ chuyển thành "Chat với Admin"
3. Gõ tin nhắn của bạn
4. Nhấn Enter hoặc click nút gửi

### Bước 4: Nhận thông báo từ Admin

Khi Admin trả lời:
- **Badge đỏ** xuất hiện trên icon chatbot (số lượng tin nhắn mới)
- **Âm thanh thông báo** (nếu bật)
- Tin nhắn tự động load vào cửa sổ chat

### Bước 5: Chuyển về chế độ Auto

Click lại icon 🤖 để quay về chế độ trả lời tự động

---

## 💡 MẸO SỬ DỤNG

### Quick Replies (Câu trả lời nhanh)

Ở chế độ Auto, bạn thấy các nút:
- **Giờ làm việc**
- **Liên hệ**
- **Đưa ra Offer**
- **Hướng dẫn bán**

👉 Click vào nút thay vì gõ!

### Từ khóa thông minh

Chatbot hiểu nhiều cách hỏi:
- "giờ làm việc" = "thời gian" = "mở cửa"
- "liên hệ" = "số điện thoại" = "hotline"
- "mua" = "đặt hàng" = "order"

### Lưu ý khi Chat với Admin

✅ **Nên:**
- Mô tả vấn đề rõ ràng
- Để lại thông tin liên hệ nếu cần
- Kiên nhẫn chờ phản hồi (thường trong 24h)

❌ **Không nên:**
- Spam tin nhắn
- Gửi nội dung không phù hợp
- Đòi hỏi phản hồi ngay lập tức

---

## 🔔 THÔNG BÁO

### Badge đỏ trên icon
- Hiển thị số lượng tin nhắn mới từ Admin
- Tự động ẩn khi bạn đọc tin nhắn

### Âm thanh
- Phát âm thanh khi có tin nhắn mới (chỉ ở chế độ Chat với Admin)
- Có thể tắt âm thanh trong trình duyệt

---

## ❓ CÂU HỎI THƯỜNG GẶP

### Q: Tôi có cần đăng nhập để chat không?
**A:** 
- **Chế độ Auto**: Không cần đăng nhập
- **Chat với Admin**: CẦN đăng nhập

### Q: Tin nhắn có được lưu lại không?
**A:** 
- **Chế độ Auto**: KHÔNG lưu (chỉ trong session)
- **Chat với Admin**: CÓ lưu vào database

### Q: Admin trả lời trong bao lâu?
**A:** Thường trong vòng 24-48 giờ làm việc

### Q: Tôi có thể xóa tin nhắn đã gửi không?
**A:** Hiện tại chưa hỗ trợ tính năng xóa tin nhắn

### Q: Chatbot có online 24/7 không?
**A:** 
- **Auto Reply**: Có, 24/7
- **Admin**: Theo giờ làm việc (8:00-22:00)

---

## 🛠️ KỸ THUẬT

### Các API được sử dụng:

```javascript
// Gửi tin nhắn đến Admin
POST /Chat/SendUserChatBotMessage
Body: { message: "..." }

// Lấy tin nhắn của tôi
GET /Chat/GetMyChatBotMessages

// Đếm tin nhắn chưa đọc
GET /Chat/GetChatBotUnreadCount
```

### Auto-refresh:
- Check tin nhắn mới mỗi **5 giây** khi ở chế độ Chat với Admin
- Dừng check khi chuyển về chế độ Auto hoặc đóng chat

---

## 🎨 GIAO DIỆN

### Icon ChatBot
- **Màu vàng**: Nút mở chatbot
- **Badge đỏ**: Tin nhắn mới
- **Hiệu ứng pulse**: Thu hút chú ý

### Cửa sổ Chat
- **Header vàng**: Logo và trạng thái
- **Icon 👨‍💼/🤖**: Chuyển đổi mode
- **Nền xám**: Khung tin nhắn
- **Nút gửi xanh**: Gửi tin nhắn

### Tin nhắn
- **Bên trái (trắng)**: Tin nhắn từ Bot/Admin
- **Bên phải (xanh)**: Tin nhắn của bạn

---

## 🚀 BẮT ĐẦU SỬ DỤNG

1. ✅ Chạy SQL script: `SQL_CreateChatBotTable.sql`
2. ✅ Build lại project
3. ✅ Mở website
4. ✅ Tìm icon chat góc phải dưới
5. ✅ Bắt đầu chat!

**Chúc bạn có trải nghiệm tuyệt vời với ChatBot! 🎉**
