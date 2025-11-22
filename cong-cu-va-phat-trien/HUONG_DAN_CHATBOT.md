# HƯỚNG DẪN CÀI ĐẶT VÀ SỬ DỤNG HỆ THỐNG CHATBOT

## 📋 TỔNG QUAN

Hệ thống ChatBot cho phép:
- ✅ Người dùng chat trực tiếp với Admin qua widget chatbot
- ✅ Admin quản lý và trả lời tất cả tin nhắn từ người dùng
- ✅ Hiển thị thông báo tin nhắn mới từ Admin
- ✅ Lưu trữ lịch sử trò chuyện

---

## 🛠️ CÀI ĐẶT

### Bước 1: Tạo bảng database

Chạy script SQL sau trong SQL Server:

```sql
-- File: SQL_CreateChatBotTable.sql
CREATE TABLE ChatBotMessages (
    idChatBot INT PRIMARY KEY IDENTITY(1,1),
    idUser INT NOT NULL,
    idAdmin INT NULL,
    messageContent NVARCHAR(2000) NOT NULL,
    dateSent DATETIME NOT NULL,
    isRead BIT NOT NULL DEFAULT 0,
    isFromAdmin BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (idUser) REFERENCES Login(idAccount),
    FOREIGN KEY (idAdmin) REFERENCES Login(idAccount)
);

CREATE INDEX IX_ChatBotMessages_User ON ChatBotMessages(idUser, dateSent DESC);
CREATE INDEX IX_ChatBotMessages_Unread ON ChatBotMessages(idUser, isRead, isFromAdmin);
```

### Bước 2: Build lại project

Trong Visual Studio:
1. Build -> Rebuild Solution (Ctrl + Shift + B)
2. Đợi build hoàn tất

---

## 🎯 CHỨC NĂNG CHO ADMIN

### 1. Truy cập Quản lý ChatBot

- Đăng nhập với tài khoản **Admin** (role 5)
- Vào menu **"Quản lý ChatBot"** trong sidebar
- Xem dashboard thống kê:
  - Tổng người dùng đã chat
  - Tin nhắn chưa đọc
  - Chat hôm nay
  - Số lượng cần trả lời

### 2. Xem danh sách người dùng

- Danh sách hiển thị tất cả user đã gửi tin nhắn
- Sắp xếp theo thời gian tin nhắn gần nhất
- Hiển thị badge đỏ cho tin nhắn chưa đọc
- Click vào user để xem chi tiết và trả lời

### 3. Trả lời tin nhắn

- Click vào user trong danh sách
- Xem lịch sử trò chuyện đầy đủ
- Nhập tin nhắn trong ô chat
- Nhấn Enter hoặc click icon gửi
- Tin nhắn sẽ được gửi ngay lập tức

---

## 👤 CHỨC NĂNG CHO NGƯỜI DÙNG (TIẾP THEO)

### Widget ChatBot

Cần tích hợp widget chatbot vào trang web:

**File cần tạo tiếp theo:**
- `Assets/_FeGit/js/chatbot-widget.js` - Widget chatbot
- Cập nhật `_TrangChu.cshtml` - Thêm widget vào layout

### API Endpoints cho người dùng:

✅ **Đã có sẵn:**
- `POST /Chat/SendUserChatBotMessage` - Gửi tin nhắn
- `GET /Chat/GetMyChatBotMessages` - Lấy tin nhắn
- `GET /Chat/GetChatBotUnreadCount` - Đếm tin nhắn chưa đọc

---

## 📊 CẤU TRÚC DATABASE

### Bảng ChatBotMessages

| Cột | Kiểu | Mô tả |
|-----|------|-------|
| idChatBot | INT | ID tin nhắn (PK) |
| idUser | INT | ID người dùng |
| idAdmin | INT | ID admin trả lời (NULL nếu từ user) |
| messageContent | NVARCHAR(2000) | Nội dung tin nhắn |
| dateSent | DATETIME | Thời gian gửi |
| isRead | BIT | Đã đọc chưa |
| isFromAdmin | BIT | Từ admin hay user |

---

## 🔐 PHÂN QUYỀN

### Admin (role 5):
- ✅ Xem tất cả tin nhắn chatbot
- ✅ Trả lời tin nhắn người dùng
- ✅ Xem thống kê

### Nhân viên (role 4):
- ✅ Xem tất cả tin nhắn chatbot
- ✅ Trả lời tin nhắn người dùng

### Người dùng (role 1, 2, 3):
- ✅ Gửi tin nhắn qua chatbot
- ✅ Nhận tin nhắn từ admin
- ✅ Xem lịch sử chat của mình

---

## 📱 MENU TRONG HỆ THỐNG

### Sidebar Admin:
```
Admin
├── Quản lý chat (Chat sản phẩm)
├── Quản lý ChatBot (Chat với Admin) ← MỚI
├── Quản lý nhân viên
└── ...
```

---

## 🎨 GIAO DIỆN

### Trang Quản lý ChatBot:
- Dashboard với 4 card thống kê
- Danh sách user với avatar tròn
- Badge đỏ hiển thị số tin nhắn chưa đọc
- Click vào user để mở chat

### Trang Chat Detail:
- Header với avatar và tên user
- Khung chat với tin nhắn cuộn
- Tin nhắn user: bên trái, nền trắng
- Tin nhắn admin: bên phải, nền gradient tím
- Ô nhập tin nhắn với nút gửi

---

## 🚀 CÁC BƯỚC TIẾP THEO

Để hoàn thiện hệ thống ChatBot, cần:

1. ✅ **Đã hoàn thành:**
   - Model và Database
   - Controller với đầy đủ API
   - Views cho Admin
   - Menu và routing

2. ⏳ **Cần làm tiếp:**
   - Widget chatbot cho người dùng (popup ở góc phải màn hình)
   - Tích hợp SignalR để real-time (không bắt buộc)
   - Thông báo âm thanh khi có tin nhắn mới
   - Auto-refresh danh sách (hoặc dùng SignalR)

---

## 📞 SUPPORT

Nếu có lỗi hoặc cần hỗ trợ:
1. Kiểm tra database đã tạo bảng chưa
2. Kiểm tra connection string `ChatContext`
3. Build lại project
4. Xem log lỗi trong Output window

---

## 🎉 HOÀN THÀNH

Hệ thống ChatBot Admin đã sẵn sàng sử dụng!
- Chạy ứng dụng
- Đăng nhập Admin
- Vào "Quản lý ChatBot"
- Bắt đầu trả lời người dùng!
