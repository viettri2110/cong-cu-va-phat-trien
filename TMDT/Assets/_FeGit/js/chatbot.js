// Chatbot Auto Reply System
(function() {
    'use strict';

    // Cấu hình chatbot
    const chatbotConfig = {
        botName: 'Trợ lý Chợ Đồ Cũ',
        welcomeMessage: 'Xin chào! Tôi là trợ lý ảo của Chợ Đồ Cũ. Tôi có thể giúp gì cho bạn? 😊',
        quickReplies: [
            'Giờ làm việc',
            'Liên hệ',
            'Đưa ra Offer',
            'Hướng dẫn bán'
        ]
    };

    // Kho câu trả lời tự động
    const autoReplies = {
        // Chào hỏi
        'chào|hello|hi|xin chào|hey': [
            'Xin chào! Rất vui được hỗ trợ bạn hôm nay! 😊',
            'Chào bạn! Tôi có thể giúp gì cho bạn?',
            'Hi! Bạn cần tôi hỗ trợ điều gì không?'
        ],
        
        // Giờ làm việc
        'giờ làm việc|thời gian làm việc|mở cửa|đóng cửa|hoạt động': [
            'Chợ Đồ Cũ hoạt động 24/7 trên website. Bộ phận chăm sóc khách hàng làm việc từ 8:00 - 22:00 hàng ngày. 🕐'
        ],
        
        // Liên hệ
        'liên hệ|liên lạc|số điện thoại|email|địa chỉ|hotline': [
            'Bạn có thể liên hệ với chúng tôi qua:\n📧 Email: chodocu@gmail.com\n📞 Hotline: 0393440859\n📍 Địa chỉ: TP. Hồ Chí Minh'
        ],
        
        // Hướng dẫn mua hàng
        'mua|mua hàng|cách mua|làm sao để mua|đặt hàng|order': [
            'Để mua hàng trên Chợ Đồ Cũ:\n1️⃣ Tìm kiếm sản phẩm\n2️⃣ Xem chi tiết và thêm vào giỏ hàng\n3️⃣ Vào giỏ hàng và tiến hành thanh toán\n4️⃣ Điền thông tin giao hàng\n5️⃣ Chọn phương thức thanh toán\nRất đơn giản! 🛒'
        ],
        
        // Hướng dẫn bán hàng
        'bán|bán hàng|cách bán|đăng bán|đăng tin|post': [
            'Để đăng bán sản phẩm:\n1️⃣ Đăng nhập tài khoản\n2️⃣ Nhấn nút "Đăng bán" ở góc trên\n3️⃣ Điền thông tin sản phẩm\n4️⃣ Tải ảnh sản phẩm\n5️⃣ Chờ duyệt từ quản trị viên\nBạn sẽ nhận được thông báo khi sản phẩm được duyệt! 📝'
        ],
        
        // Thanh toán
        'thanh toán|payment|pay|paypal|tiền': [
            'Chúng tôi hỗ trợ các phương thức thanh toán:\n💳 PayPal\n💰 COD (Thanh toán khi nhận hàng)\n🏦 Chuyển khoản ngân hàng\nTất cả đều an toàn và bảo mật!'
        ],
        
        // Vận chuyển
        'vận chuyển|ship|giao hàng|shipping|delivery': [
            'Thông tin vận chuyển:\n📦 Giao hàng toàn quốc\n⚡ Giao hàng nhanh trong 2-5 ngày\n💵 Phí ship tùy theo khu vực\n📍 Theo dõi đơn hàng realtime\nShipper sẽ liên hệ bạn trước khi giao!'
        ],
        
        // Đăng ký / Đăng nhập
        'đăng ký|đăng nhập|tài khoản|account|register|login|sign up': [
            'Để sử dụng đầy đủ tính năng:\n👤 Nhấn "Đăng nhập" góc trên cùng\n📝 Chọn "Đăng ký" nếu chưa có tài khoản\n✉️ Nhập thông tin email và mật khẩu\nRất nhanh gọn thôi! 🚀'
        ],
        
        // Bảo hành / Đổi trả
        'bảo hành|đổi trả|return|warranty|hoàn tiền': [
            'Chính sách đổi trả:\n✅ Đổi trả trong 3 ngày nếu sản phẩm lỗi\n📸 Cần có video unbox khi nhận hàng\n💯 Hoàn tiền 100% nếu shop giao sai\n🤝 Liên hệ hotline để được hỗ trợ tốt nhất!'
        ],
        
        // Tìm kiếm sản phẩm
        'tìm|tìm kiếm|search|sản phẩm|đồ|hàng': [
            'Bạn có thể tìm kiếm sản phẩm bằng cách:\n🔍 Dùng thanh tìm kiếm ở đầu trang\n📂 Lọc theo danh mục\n💰 Lọc theo giá\n📍 Lọc theo khu vực\nHãy thử ngay! 🎯'
        ],
        
        // Cảm ơn
        'cảm ơn|thank|thanks|cám ơn|tks': [
            'Không có gì! Rất vui được giúp bạn! 😊',
            'Cảm ơn bạn đã sử dụng dịch vụ! Chúc bạn mua sắm vui vẻ! 🛍️',
            'Luôn sẵn sàng hỗ trợ bạn! Hẹn gặp lại! 👋'
        ],
        
        // Tạm biệt
        'bye|tạm biệt|goodbye|chào|hẹn gặp lại': [
            'Tạm biệt! Chúc bạn một ngày tốt lành! 👋',
            'Hẹn gặp lại bạn! Mua sắm vui vẻ nhé! 😊'
        ],

        // Giá cả
        'giá|price|bao nhiêu|giá bao nhiêu': [
            'Giá sản phẩm trên Chợ Đồ Cũ rất hợp lý và cạnh tranh! 💰\nMỗi sản phẩm có giá riêng, bạn có thể:\n🔍 Tìm kiếm sản phẩm cụ thể\n💬 Liên hệ người bán để thương lượng\n📊 Sử dụng bộ lọc giá để tìm sản phẩm phù hợp'
        ],

        // Offer / Trả giá
        'offer|trả giá|đề nghị giá|chào giá|thương lượng|giảm giá|hạ giá': [
            '💰 Bạn muốn đưa ra mức giá offer cho sản phẩm?\n\nĐể đưa ra offer:\n1️⃣ Vào trang chi tiết sản phẩm\n2️⃣ Nhấn nút "Đưa ra offer"\n3️⃣ Nhập mức giá bạn mong muốn\n4️⃣ Gửi offer\n\n✅ Người bán sẽ nhận được thông báo và sẽ phản hồi sớm nhất!\n⏰ Thời gian phản hồi thường trong vòng 24h\n💬 Bạn cũng có thể chat trực tiếp với người bán để thương lượng tốt hơn!'
        ],

        // Đợi người bán phản hồi
        'chờ người bán|người bán chưa trả lời|offer chưa được duyệt|đang chờ|pending': [
            '⏳ Offer của bạn đã được gửi thành công!\n\n📨 Người bán sẽ nhận được thông báo ngay lập tức\n⏰ Thời gian phản hồi: 24-48 giờ\n🔔 Bạn sẽ nhận được thông báo khi người bán phản hồi\n\n💡 Mẹo:\n• Kiểm tra mục "Thông báo" thường xuyên\n• Đảm bảo email được bật để nhận thông báo\n• Có thể chat trực tiếp với người bán để nhanh hơn\n\nCảm ơn bạn đã kiên nhẫn! 😊'
        ],

        // Người bán không phản hồi
        'người bán không trả lời|không phản hồi|quá lâu|lâu quá': [
            '😔 Rất tiếc vì sự chậm trễ này!\n\nNếu người bán không phản hồi sau 48h:\n📞 Liên hệ hotline: 0393440859\n📧 Email: chodocu@gmail.com\n💬 Chat với admin để được hỗ trợ\n\nChúng tôi sẽ:\n✅ Nhắc nhở người bán\n✅ Tìm sản phẩm tương tự cho bạn\n✅ Đảm bảo quyền lợi của bạn\n\nXin lỗi vì sự bất tiện! 🙏'
        ],

        // Hủy offer
        'hủy offer|không muốn mua nữa|cancel offer|xóa offer': [
            '🚫 Để hủy offer của bạn:\n\n1️⃣ Vào mục "Đơn hàng của tôi"\n2️⃣ Tab "Offer đang chờ"\n3️⃣ Chọn offer muốn hủy\n4️⃣ Nhấn "Hủy offer"\n\n⚠️ Lưu ý:\n• Chỉ hủy được khi người bán chưa chấp nhận\n• Sau khi hủy không thể hoàn tác\n\nBạn có chắc muốn hủy không? 🤔'
        ]
    };

    // Biến lưu trạng thái
    let chatHistory = [];
    let isTyping = false;
    let isAdminMode = false; // Chế độ chat với admin

    // Khởi tạo chatbot
    function initChatbot() {
        const chatbotHTML = `
            <div class="chatbot-container">
                <button class="chatbot-toggle-btn" id="chatbotToggle">
                    <i class="fas fa-comments"></i>
                    <i class="fas fa-times"></i>
                    <span class="chatbot-notification"></span>
                </button>
                
                <div class="chatbot-window" id="chatbotWindow">
                    <div class="chatbot-header">
                        <div class="chatbot-header-title">
                            <div class="chatbot-avatar">
                                <i class="fas fa-robot" style="color: #ffba00;"></i>
                            </div>
                            <div>
                                <h3 id="chatbotTitle">${chatbotConfig.botName}</h3>
                                <p id="chatbotStatus">Trực tuyến</p>
                            </div>
                        </div>
                        <button class="chatbot-admin-btn" id="switchToAdminBtn" title="Chat với Admin" style="display:none;">
                            <i class="fas fa-user-headset"></i>
                        </button>
                        <button class="chatbot-admin-btn" id="backToBotBtn" style="display:none;" title="Quay lại chatbot">
                            <i class="fas fa-robot"></i>
                        </button>
                    </div>
                    
                    <div class="chatbot-messages" id="chatbotMessages">
                        <div class="message bot">
                            <div class="message-avatar">
                                <i class="fas fa-robot"></i>
                            </div>
                            <div class="message-content">
                                ${chatbotConfig.welcomeMessage}
                            </div>
                        </div>
                        
                        <div class="typing-indicator" id="typingIndicator">
                            <div class="message-avatar" style="background: linear-gradient(135deg, #ffba00 0%, #ff9800 100%); color: white;">
                                <i class="fas fa-robot"></i>
                            </div>
                            <div class="typing-dots">
                                <span></span>
                                <span></span>
                                <span></span>
                            </div>
                        </div>
                    </div>
                    
                    <div class="chatbot-quick-replies" id="quickReplies">
                        ${chatbotConfig.quickReplies.map(reply => 
                            `<button class="quick-reply-btn" data-text="${reply}">${reply}</button>`
                        ).join('')}
                    </div>
                    
                    <div class="chatbot-input">
                        <input type="text" id="chatbotInput" placeholder="Nhập tin nhắn..." />
                        <button class="chatbot-send-btn" id="chatbotSend">
                            <i class="fas fa-paper-plane"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', chatbotHTML);
        attachEventListeners();
        
        // Kiểm tra lịch sử chat với admin
        checkAdminChatHistory();
    }

    // Kiểm tra đăng nhập
    function checkLoginStatus() {
        // Kiểm tra xem có session đăng nhập không (bằng cách gọi API test)
        return $.ajax({
            url: '/Chat/CheckLoginStatus',
            method: 'GET',
            async: false
        }).responseJSON?.isLoggedIn || false;
    }

    // Gắn sự kiện
    function attachEventListeners() {
        const toggleBtn = document.getElementById('chatbotToggle');
        const chatbotWindow = document.getElementById('chatbotWindow');
        const sendBtn = document.getElementById('chatbotSend');
        const input = document.getElementById('chatbotInput');
        const quickReplyBtns = document.querySelectorAll('.quick-reply-btn');

        toggleBtn.addEventListener('click', () => {
            // Kiểm tra đăng nhập bằng cách check text của link
            const loginLink = document.querySelector('.headerBottom__login a');
            const linkText = loginLink ? loginLink.textContent.trim() : '';
            
            // Nếu text là "Đăng nhập" thì chưa đăng nhập
            // Nếu text là "Thông tin" thì đã đăng nhập
            const isLoggedIn = linkText.includes('Thông tin');
            
            if (!isLoggedIn) {
                // Chưa đăng nhập → Hiển thị thông báo và chuyển đến trang đăng nhập
                if (confirm('Bạn cần đăng nhập để sử dụng chatbot. Đăng nhập ngay?')) {
                    window.location.href = '/Login/DangNhap';
                }
                return;
            }
            
            // Đã đăng nhập → Mở chatbot
            toggleBtn.classList.toggle('active');
            chatbotWindow.classList.toggle('active');
            if (chatbotWindow.classList.contains('active')) {
                input.focus();
            }
        });

        sendBtn.addEventListener('click', () => sendMessage());
        input.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                sendMessage();
            }
        });

        quickReplyBtns.forEach(btn => {
            btn.addEventListener('click', () => {
                const text = btn.getAttribute('data-text');
                sendMessage(text);
            });
        });

        // Nút chuyển sang chat với admin
        const switchToAdminBtn = document.getElementById('switchToAdminBtn');
        if (switchToAdminBtn) {
            switchToAdminBtn.addEventListener('click', () => {
                switchToAdminMode();
            });
        }

        // Nút quay lại chatbot
        const backToBotBtn = document.getElementById('backToBotBtn');
        if (backToBotBtn) {
            backToBotBtn.addEventListener('click', () => {
                backToBotMode();
            });
        }
    }

    // Gửi tin nhắn
    function sendMessage(text) {
        const input = document.getElementById('chatbotInput');
        const message = text || input.value.trim();

        if (!message) return;

        // Hiển thị tin nhắn người dùng
        addMessage(message, 'user');
        input.value = '';

        // Nếu đang ở chế độ admin, chỉ gửi lên server (không auto-reply)
        if (isAdminMode) {
            sendToAdmin(message);
            return;
        }

        // Chế độ bot: Kiểm tra xem có phải khiếu nại/cần admin không
        const needsAdmin = detectComplaint(message);
        
        if (needsAdmin) {
            // Gửi tin nhắn đến admin qua API
            sendToAdmin(message);
        } else {
            // Trả lời tự động
            setTimeout(() => {
                const reply = getAutoReply(message);
                showTypingIndicator();
                
                setTimeout(() => {
                    hideTypingIndicator();
                    addMessage(reply, 'bot');
                }, 1000 + Math.random() * 1000);
            }, 500);
        }
    }

    // Phát hiện khiếu nại/phàn nàn
    function detectComplaint(message) {
        const complainKeywords = [
            'khiếu nại', 'phàn nàn', 'tố cáo', 'báo cáo',
            'lừa đảo', 'gian lận', 'không nhận được hàng',
            'sản phẩm lỗi', 'kém chất lượng', 'không như mô tả',
            'yêu cầu hoàn tiền', 'hoàn tiền', 'đền bù',
            'không giao hàng', 'người bán không trả lời',
            'admin', 'quản trị viên', 'liên hệ admin',
            'cần hỗ trợ', 'cần giúp đỡ', 'có vấn đề',
            'tệ', 'tệ quá', 'quá tệ', 'dở', 'kém', 'thất vọng',
            'không hài lòng', 'không ổn', 'tồi', 'không tốt',
            'rất tệ', 'quá dở', 'chán', 'thất bại',
            'lẽ ra', 'có thể', 'giúp', 'hỗ trợ', 'sao',
            'tại sao', 'vì sao', 'làm sao', 'thế nào'
        ];

        const messageLower = message.toLowerCase();
        return complainKeywords.some(keyword => messageLower.includes(keyword));
    }

    // Gửi tin nhắn đến admin
    function sendToAdmin(message) {
        showTypingIndicator();

        // Gọi API để lưu tin nhắn vào database
        $.ajax({
            url: '/Chat/SendChatBotMessage',
            method: 'POST',
            data: { message: message },
            xhrFields: {
                withCredentials: true  // Đảm bảo gửi cookies/session
            },
            success: function(response) {
                hideTypingIndicator();
                
                if (response.success) {
                    // Cập nhật lastMessageId để tránh load lại tin nhắn vừa gửi
                    if (response.messageId) {
                        lastMessageId = response.messageId;
                    }
                    
                    if (isAdminMode) {
                        // Chế độ admin: Không hiện gì, chỉ đợi admin phản hồi qua polling
                        return;
                    } else if (response.isComplaint) {
                        // Đánh dấu đã phàn nàn - cho phép chuyển sang chat admin
                        hasComplained = true;
                        
                        // Hiện nút chuyển admin và thông báo
                        const switchBtn = document.getElementById('switchToAdminBtn');
                        if (switchBtn) {
                            switchBtn.style.display = 'inline-block';
                            switchBtn.classList.add('pulse-animation');
                        }
                        
                        // Chỉ hiện reply bot nếu có (tin nhắn đầu tiên)
                        if (response.botResponse) {
                            addMessage(response.botResponse, 'bot');
                        }
                        
                        // Tin nhắn khiếu nại đã được ghi nhận
                        addMessage('🔔 Tin nhắn của bạn đã được chuyển đến bộ phận hỗ trợ. Admin sẽ liên hệ với bạn trong thời gian sớm nhất! 📞', 'bot');
                        
                        // Hiển thị thông tin liên hệ và hướng dẫn
                        setTimeout(() => {
                            addMessage('💬 Bạn có thể click vào nút <i class="fas fa-user-headset"></i> ở góc trên để chat trực tiếp với admin ngay bây giờ!\n\nHoặc liên hệ:\n📧 Email: chodocu@gmail.com\n📞 Hotline: 0393440859', 'bot');
                        }, 1500);
                    } else {
                        // Bot trả lời tự động (chỉ tin nhắn đầu tiên)
                        if (response.botResponse) {
                            addMessage(response.botResponse, 'bot');
                        }
                    }
                } else {
                    addMessage('⚠️ Xin lỗi, có lỗi xảy ra. Vui lòng thử lại hoặc liên hệ hotline: 0393440859', 'bot');
                }
            },
            error: function(xhr) {
                hideTypingIndicator();
                console.error('❌ Lỗi gửi tin nhắn:', xhr);
                console.error('Status:', xhr.status);
                console.error('Response:', xhr.responseText);
                console.error('Response JSON:', xhr.responseJSON);
                
                // Kiểm tra phản hồi JSON
                if (xhr.responseJSON && !xhr.responseJSON.success) {
                    // Server trả về JSON với success = false
                    if (xhr.responseJSON.message && xhr.responseJSON.message.includes('đăng nhập')) {
                        addMessage('⚠️ Phiên đăng nhập đã hết hạn. Vui lòng <a href="/Login/DangNhap" style="color: #007bff; text-decoration: underline;">đăng nhập lại</a> để tiếp tục! 🔐', 'bot');
                    } else {
                        addMessage('⚠️ ' + (xhr.responseJSON.message || 'Có lỗi xảy ra'), 'bot');
                    }
                } else if (xhr.status === 401 || xhr.status === 403) {
                    addMessage('⚠️ Phiên đăng nhập đã hết hạn. Vui lòng <a href="/Login/DangNhap" style="color: #007bff; text-decoration: underline;">đăng nhập lại</a>! 🔐', 'bot');
                } else if (xhr.status === 500) {
                    addMessage('⚠️ Lỗi server. Vui lòng liên hệ hotline: 0393440859 📞', 'bot');
                } else {
                    addMessage('⚠️ Không thể gửi tin nhắn (Mã lỗi: ' + xhr.status + '). Vui lòng thử lại sau hoặc liên hệ hotline: 0393440859 📞', 'bot');
                }
            }
        });
    }

    // Tìm câu trả lời phù hợp
    function getAutoReply(userMessage) {
        const messageLower = userMessage.toLowerCase();
        
        for (const [pattern, replies] of Object.entries(autoReplies)) {
            const keywords = pattern.split('|');
            if (keywords.some(keyword => messageLower.includes(keyword))) {
                const replyArray = Array.isArray(replies) ? replies : [replies];
                return replyArray[Math.floor(Math.random() * replyArray.length)];
            }
        }

        // Câu trả lời mặc định
        const defaultReplies = [
            'Xin lỗi, tôi chưa hiểu câu hỏi của bạn. Bạn có thể diễn đạt lại được không? 😊',
            'Tôi chưa có thông tin về vấn đề này. Vui lòng liên hệ hotline: 0393440859 để được hỗ trợ tốt hơn! 📞',
            'Câu hỏi của bạn hơi khó đây! Bạn có thể thử hỏi về: giờ làm việc, liên hệ, hướng dẫn mua bán, thanh toán... 🤔'
        ];
        
        return defaultReplies[Math.floor(Math.random() * defaultReplies.length)];
    }

    // Thêm tin nhắn vào chat
    function addMessage(text, sender) {
        const messagesContainer = document.getElementById('chatbotMessages');
        const time = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
        
        const messageHTML = `
            <div class="message ${sender}">
                <div class="message-avatar">
                    <i class="fas fa-${sender === 'bot' ? 'robot' : 'user'}"></i>
                </div>
                <div class="message-content">
                    ${text.replace(/\n/g, '<br>')}
                </div>
            </div>
        `;

        const typingIndicator = document.getElementById('typingIndicator');
        messagesContainer.insertBefore(
            createElementFromHTML(messageHTML),
            typingIndicator
        );

        messagesContainer.scrollTop = messagesContainer.scrollHeight;
        chatHistory.push({ text, sender, time });
    }

    // Hiển thị indicator đang gõ
    function showTypingIndicator() {
        const indicator = document.getElementById('typingIndicator');
        indicator.classList.add('active');
        isTyping = true;
        scrollToBottom();
    }

    // Ẩn indicator đang gõ
    function hideTypingIndicator() {
        const indicator = document.getElementById('typingIndicator');
        indicator.classList.remove('active');
        isTyping = false;
    }

    // Cuộn xuống cuối
    function scrollToBottom() {
        const messagesContainer = document.getElementById('chatbotMessages');
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    // Tạo element từ HTML string
    function createElementFromHTML(htmlString) {
        const div = document.createElement('div');
        div.innerHTML = htmlString.trim();
        return div.firstChild;
    }

    // Biến để lưu interval ID và ID tin nhắn cuối cùng
    let pollingInterval = null;
    let lastMessageId = 0;
    let hasComplained = false; // Theo dõi xem user đã phàn nàn chưa
    
    // Kiểm tra localStorage xem đã từng chat với admin chưa
    function checkAdminChatHistory() {
        const hasChattedWithAdmin = localStorage.getItem('hasChattedWithAdmin');
        if (hasChattedWithAdmin === 'true') {
            hasComplained = true;
            const switchBtn = document.getElementById('switchToAdminBtn');
            if (switchBtn) {
                switchBtn.style.display = 'inline-block';
            }
        }
    }

    // Chuyển sang chế độ chat với admin
    function switchToAdminMode() {
        // Kiểm tra xem user đã phàn nàn chưa
        if (!hasComplained) {
            addMessage('⚠️ Bạn cần gửi phản hồi về sản phẩm hoặc dịch vụ trước khi chat với admin.\n\n💡 Hãy cho chúng tôi biết vấn đề bạn gặp phải!', 'bot');
            return;
        }
        
        // Lưu trạng thái đã chat với admin
        localStorage.setItem('hasChattedWithAdmin', 'true');
        isAdminMode = true;
        
        // Cập nhật giao diện
        document.getElementById('chatbotTitle').textContent = 'Chat với Admin';
        document.getElementById('chatbotStatus').textContent = 'Hỗ trợ trực tiếp';
        
        const avatar = document.querySelector('.chatbot-header .chatbot-avatar i');
        avatar.className = 'fas fa-user-headset';
        avatar.style.color = '#28a745';
        
        // Ẩn quick replies (chỉ dùng cho bot)
        const quickReplies = document.getElementById('quickReplies');
        if (quickReplies) quickReplies.style.display = 'none';
        
        // Đổi nút: Ẩn switch, hiện back
        const switchBtn = document.getElementById('switchToAdminBtn');
        if (switchBtn) switchBtn.style.display = 'none';
        
        const backBtn = document.getElementById('backToBotBtn');
        if (backBtn) backBtn.style.display = 'inline-block';
        
        // Xóa tin nhắn cũ và load lịch sử chat với admin
        clearMessages();
        addMessage('🔄 Đang tải lịch sử chat với admin...', 'bot');
        
        loadAdminChatHistory();
        
        // Bắt đầu polling để nhận tin nhắn mới từ admin
        startPolling();
    }

    // Quay lại chế độ chatbot tự động
    function backToBotMode() {
        isAdminMode = false;
        
        // Dừng polling
        stopPolling();
        
        // Cập nhật giao diện
        document.getElementById('chatbotTitle').textContent = 'ChoDoCu Bot';
        document.getElementById('chatbotStatus').textContent = 'Trợ lý ảo';
        
        const avatar = document.querySelector('.chatbot-header .chatbot-avatar i');
        avatar.className = 'fas fa-robot';
        avatar.style.color = '#ffba00';
        
        // Hiển thị lại quick replies
        const quickReplies = document.getElementById('quickReplies');
        if (quickReplies) quickReplies.style.display = 'flex';
        
        // Đổi nút: Hiện switch, ẩn back
        const switchBtn = document.getElementById('switchToAdminBtn');
        if (switchBtn) switchBtn.style.display = 'inline-block';
        
        const backBtn = document.getElementById('backToBotBtn');
        if (backBtn) backBtn.style.display = 'none';
        
        // Xóa lịch sử và hiển thị tin nhắn chào mừng bot
        clearMessages();
        addMessageWithoutSave('👋 Xin chào! Tôi là trợ lý ảo của ChoDoCu. Bạn cần hỗ trợ gì?', 'bot');
    }

    // Xóa tin nhắn
    function clearMessages() {
        const messagesContainer = document.getElementById('chatbotMessages');
        const existingMessages = messagesContainer.querySelectorAll('.message:not(#typingIndicator)');
        existingMessages.forEach(msg => msg.remove());
        chatHistory = [];
    }

    // Load lịch sử chat với admin từ database
    function loadAdminChatHistory() {
        $.ajax({
            url: '/Chat/GetMyChatBotMessages',
            method: 'GET',
            success: function(response) {
                // Xóa tin nhắn loading
                clearMessages();
                
                if (response.success && response.messages && response.messages.length > 0) {
                    // Hiển thị lịch sử chat
                    response.messages.forEach(msg => {
                        const sender = msg.isFromAdmin ? 'admin' : 'user';
                        addMessageWithoutSave(msg.content, sender);
                    });
                    
                    // Lưu ID tin nhắn cuối cùng
                    lastMessageId = response.messages[response.messages.length - 1].id;
                } else {
                    addMessageWithoutSave('📝 Chào bạn! Đây là hệ thống hỗ trợ trực tiếp. Admin sẽ phản hồi trong vài phút. Hãy gửi tin nhắn của bạn!', 'admin');
                }
            },
            error: function(xhr) {
                clearMessages();
                addMessageWithoutSave('⚠️ Không thể tải lịch sử chat. Vui lòng thử lại!', 'bot');
                console.error('Lỗi load chat history:', xhr);
            }
        });
    }

    // Thêm tin nhắn vào chat (không lưu vào history array)
    function addMessageWithoutSave(text, sender) {
        const messagesContainer = document.getElementById('chatbotMessages');
        
        // Xác định icon, màu và avatar dựa trên sender
        let avatarHTML;
        if (sender === 'admin') {
            avatarHTML = `
                <div class="message-avatar" style="background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white;">
                    <i class="fas fa-user-tie"></i>
                </div>`;
        } else if (sender === 'bot') {
            avatarHTML = `
                <div class="message-avatar" style="background: linear-gradient(135deg, #ffba00 0%, #ff9800 100%); color: white;">
                    <i class="fas fa-robot"></i>
                </div>`;
        } else {
            avatarHTML = `
                <div class="message-avatar" style="background: #e0e0e0; color: #666;">
                    <i class="fas fa-user"></i>
                </div>`;
        }
        
        const messageHTML = `
            <div class="message ${sender === 'user' ? 'user' : 'bot'}">
                ${avatarHTML}
                <div class="message-content">
                    ${text.replace(/\n/g, '<br>')}
                </div>
            </div>
        `;

        const typingIndicator = document.getElementById('typingIndicator');
        messagesContainer.insertBefore(
            createElementFromHTML(messageHTML),
            typingIndicator
        );

        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    // Bắt đầu polling (kiểm tra tin nhắn mới mỗi 3 giây)
    function startPolling() {
        // Dừng polling cũ nếu có
        stopPolling();
        
        // Bắt đầu polling mới
        pollingInterval = setInterval(checkNewMessages, 3000);
    }

    // Dừng polling
    function stopPolling() {
        if (pollingInterval) {
            clearInterval(pollingInterval);
            pollingInterval = null;
        }
    }

    // Kiểm tra tin nhắn mới từ admin
    function checkNewMessages() {
        if (!isAdminMode) {
            stopPolling();
            return;
        }

        $.ajax({
            url: '/Chat/GetMyChatBotMessages',
            method: 'GET',
            success: function(response) {
                if (response.success && response.messages && response.messages.length > 0) {
                    // Lọc tin nhắn mới (ID lớn hơn lastMessageId)
                    const newMessages = response.messages.filter(msg => msg.id > lastMessageId);
                    
                    if (newMessages.length > 0) {
                        // Hiển thị tin nhắn mới
                        newMessages.forEach(msg => {
                            const sender = msg.isFromAdmin ? 'admin' : 'user';
                            addMessageWithoutSave(msg.content, sender);
                        });
                        
                        // Cập nhật lastMessageId
                        lastMessageId = response.messages[response.messages.length - 1].id;
                    }
                }
            },
            error: function(xhr) {
                console.error('Lỗi kiểm tra tin nhắn mới:', xhr);
            }
        });
    }

    // Khởi động khi DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initChatbot);
    } else {
        initChatbot();
    }

})();
