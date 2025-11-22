using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMDT.Models
{
    /// <summary>
    /// Model cho tin nhắn ChatBot (chat với admin)
    /// </summary>
    [Table("ChatBotMessages")]
    public class ChatBotMessage
    {
        [Key]
        public int idChatBot { get; set; }
        
        public int idUser { get; set; }
        
        public int? idAdmin { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string messageContent { get; set; }
        
        public DateTime dateSent { get; set; }
        
        public bool isRead { get; set; }
        
        public bool isFromAdmin { get; set; }
    }

    /// <summary>
    /// ViewModel để hiển thị danh sách người dùng chat với chatbot
    /// </summary>
    public class ChatBotUserViewModel
    {
        public int idUser { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string lastMessage { get; set; }
        public DateTime? lastMessageDate { get; set; }
        public int unreadCount { get; set; }
    }

    /// <summary>
    /// ViewModel để hiển thị tin nhắn chatbot
    /// </summary>
    public class ChatBotMessageViewModel
    {
        public int idChatBot { get; set; }
        public int idUser { get; set; }
        public int? idAdmin { get; set; }
        public string messageContent { get; set; }
        public DateTime dateSent { get; set; }
        public bool isRead { get; set; }
        public bool isFromAdmin { get; set; }
        public bool isMyMessage { get; set; }
        public string senderName { get; set; }
    }
}
