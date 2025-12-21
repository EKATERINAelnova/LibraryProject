using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{
    public class BooksCopyInfo
    {
        [Key]
        [Column("copy_id")]
        public int CopyId { get; set; }

        [Column("book_title"), MaxLength(200)]
        public string BookTitle { get; set; } = string.Empty;

        [Column("publisher"), MaxLength(150)]
        public string Publisher { get; set; } = string.Empty;

        [Column("year_publish")]
        public int? YearPublish { get; set; }

        [Column("country"), MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Column("section"), MaxLength(100)]
        public string Section { get; set; } = string.Empty;

        [Column("status"), MaxLength(20)]
        public string Status { get; set; } = string.Empty;
        [Column("closet")]
        public int Closet { get; set; }

        [Column("shelf")]
        public int Shelf { get; set; }

        [Column("place")]
        public int Place { get; set; }

        [Column("authors"), MaxLength(500)]
        public string Authors { get; set; } = string.Empty;

        [Column("genres"), MaxLength(500)]
        public string Genres { get; set; } = string.Empty;
    }
}
