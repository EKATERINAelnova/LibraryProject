using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("requests")]
    public class Requests
    {
        [Key]
        [Column("request_id")]
        public int RequestId { get; set; }

        [Column("reader_id")]
        public int ReaderId { get; set; }

        [Column("copy_id")]
        public int CopyBookId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("status"), MaxLength(20)]
        public string Status { get; set; } = "выдана";

        [ForeignKey("CopyId")]
        public CopyBooks? CopyBook { get; set; }

        [ForeignKey("ReaderId")]
        public Readers? Reader { get; set; }
    }
}
