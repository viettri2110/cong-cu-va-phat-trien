using System.Data.Entity;

namespace TMDT.Models
{
    public class ChatContext : DbContext
    {
        public ChatContext()
            : base("name=ChatContext")
        {
            // Tắt lazy loading và proxy creation để tránh lỗi
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatBotMessage> ChatBotMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Cấu hình ChatRoom
            modelBuilder.Entity<ChatRoom>()
                .ToTable("ChatRooms")
                .HasKey(c => c.idRoom);
            
            modelBuilder.Entity<ChatRoom>()
                .Property(c => c.idRoom)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            // Cấu hình ChatMessage
            modelBuilder.Entity<ChatMessage>()
                .ToTable("ChatMessages")
                .HasKey(c => c.idMessage);
                
            modelBuilder.Entity<ChatMessage>()
                .Property(c => c.idMessage)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            // Cấu hình ChatBotMessage
            modelBuilder.Entity<ChatBotMessage>()
                .ToTable("ChatBotMessages")
                .HasKey(c => c.idChatBot);
                
            modelBuilder.Entity<ChatBotMessage>()
                .Property(c => c.idChatBot)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
        }
    }
}
