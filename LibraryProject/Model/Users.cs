using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{

    [Table("users")]
    public class Users
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("login"), Required, MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        [Column("email"), Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Column("password_hash"), Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("role")]
        [MaxLength(20)]
        public string Role { get; set; } = "reader";

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public virtual Workers? Worker { get; set; }
        public virtual Readers? Reader { get; set; }
    }
}
