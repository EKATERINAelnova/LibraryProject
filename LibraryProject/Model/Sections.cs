using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("sections")]
    public class Sections
    {
        [Key]
        [Column("section_id")]
        public int SectionId { get; set; }

        [Column("name"), MaxLength(50)]
        public string? Name { get; set; }

    }
}
