using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("copy_status")]
    public class CopyStatus
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("name"), MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
