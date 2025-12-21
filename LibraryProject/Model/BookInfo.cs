using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("books_info")]
    public class BookInfo
    {
        [Key]
        [Column("book_id")]
        public int BookId { get; set; }

        [Column("title"), Required, MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [Column("authors")]
        public string? Authors { get; set; } = string.Empty;

        [Column("genres"), MaxLength(100)]
        public string? Genres { get; set; } = string.Empty;

        [Column("book_count")]
        public int Count { get; set; }
    }

}
