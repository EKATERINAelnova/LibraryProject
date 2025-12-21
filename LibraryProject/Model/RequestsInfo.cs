using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table ("all_user_requests_info")]
    public class RequestsInfo
    {
        [Key]
        [Column("request_id")]
        public int RequestId { get; set; }
        [Column("reader_id")]
        public int UserId { get; set; }
        [Column("reader_name")]
        public string? UserName { get; set; }

        [Column("copy_id")]
        public int CopyId { get; set; }
        [Column("book_id")]
        public int BookId { get; set; }

        [Column("book_title"), Required, MaxLength(250)]
        public string BookTitle { get; set; }

        [Column("authors")]
        public string? Authors { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime RequestDate { get; set; }

        [Column("status"), MaxLength(20)]
        public string? Status { get; set; }
    }
}
