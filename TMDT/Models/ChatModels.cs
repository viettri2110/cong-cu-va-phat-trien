using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMDT.Models
{
    [Table("ChatRooms")]
    public class ChatRoom
    {
        [Key]
        public int idRoom { get; set; }
        
        public int idProduct { get; set; }
        
        public int idBuyer { get; set; }
        
        public int idSeller { get; set; }
        
        public DateTime dateCreated { get; set; }
        
        public DateTime? lastMessageDate { get; set; }
        
        public bool isActive { get; set; }
    }

    [Table("ChatMessages")]
    public class ChatMessage
    {
        [Key]
        public int idMessage { get; set; }
        
        public int idRoom { get; set; }
        
        public int idSender { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string messageContent { get; set; }
        
        public DateTime dateSent { get; set; }
        
        public bool isRead { get; set; }
        
        [StringLength(20)]
        public string messageType { get; set; }
    }

    // ViewModel để hiển thị danh sách chat
    public class ChatRoomViewModel
    {
        public int idRoom { get; set; }
        public int idProduct { get; set; }
        public string productName { get; set; }
        public string productImage { get; set; }
        public decimal productPrice { get; set; }
        public int otherUserId { get; set; }
        public string otherUserName { get; set; }
        public string otherUserEmail { get; set; }
        public string lastMessage { get; set; }
        public DateTime? lastMessageDate { get; set; }
        public int unreadCount { get; set; }
    }

    // ViewModel để hiển thị tin nhắn
    public class ChatMessageViewModel
    {
        public int idMessage { get; set; }
        public int idSender { get; set; }
        public string senderName { get; set; }
        public string messageContent { get; set; }
        public DateTime dateSent { get; set; }
        public bool isRead { get; set; }
        public bool isMyMessage { get; set; }
    }
}
