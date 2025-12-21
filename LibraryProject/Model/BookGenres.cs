using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("book_genres")]
    public class BookGenres
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("book_id")]
        public int BookId { get; set; }

        [Column("genre_id")]
        public int GenreId { get; set; }

        [ForeignKey("BookId")]
        public Books? Book { get; set; }

        [ForeignKey("GenreId")]
        public Genres? Genre { get; set; }
    }
}
