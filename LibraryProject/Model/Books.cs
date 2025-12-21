using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{
    [Table("books")]
    public class Books
    {
        [Key]
        [Column("book_id")]
        public int BookId { get; set; }

        [Column("title"), Required, MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [Column("first_year_publish")]
        public int? Year { get; set; }

        [Column("annotation")]
        public string? Annotation { get; set; }
    }
}
