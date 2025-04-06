using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        
        public bool Emailconfirmed {get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public ICollection<Logs> Logs { get; set; }

    }
}