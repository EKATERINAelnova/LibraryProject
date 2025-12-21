using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("readers")]
    public class Readers
    {
        [Key, ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [MaxLength(100)]
        [Column("address")]
        public string? Address { get; set; }

        [Required, MaxLength(15)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        public virtual Users User { get; set; } = null!;
    }
}
