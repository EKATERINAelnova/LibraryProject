using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("publishers")]
    public class Publishers
    {
        [Key]
        [Column("publisher_id")]
        public int PublisherId { get; set; }

        [Column("name"), MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Column("country_id")]
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Countries? Country { get; set; }
    }
}
