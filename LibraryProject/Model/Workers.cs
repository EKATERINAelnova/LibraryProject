using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("workers")]
    public class Workers
    {
        [Key, ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(1)]
        [Column("gender")]
        public string? Gender { get; set; } // м / ж

        [Required, MaxLength(20)]
        [Column("passport")]
        public string Passport { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        [Column("inn")]
        public string Inn { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("position")]
        public string? Position { get; set; } = "библиотекарь";

        [Column("salary")]
        public int? Salary { get; set; }

        [MaxLength(100)]
        [Column("address")]
        public string? Address { get; set; }

        [MaxLength(15)]
        [Column("phone_number")]
        public string? PhoneNumber { get; set; }
        public virtual Users User { get; set; } = null!;
    }
}
