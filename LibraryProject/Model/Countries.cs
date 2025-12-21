using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("countries")]
    public class Countries
    {
        [Key]
        [Column("country_id")]
        public int Id { get; set; }

        [Column("name"), MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
