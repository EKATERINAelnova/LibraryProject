using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("book_authors")]
    public class BookAuthors
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("book_id")]
        public int BookId { get; set; }

        [Column("author_id")]
        public int AuthorId { get; set; }

        [ForeignKey("BookId")]
        public Books? Book { get; set; }

        [ForeignKey("AuthorId")]
        public Authors? Author { get; set; }
    }
}
