using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{
    [Table("genres")]
    public class Genres
    {
        [Key]
        [Column("genre_id")]
        public int GenreId { get; set; }

        [Column("name"), MaxLength(50), Required]
        public string Name { get; set; } = string.Empty;

        [Column("section_id")]
        public int SectionId { get; set; }

        // Навигационное свойство к разделу
        public Sections Section { get; set; } = null!;

        // Навигационное свойство Many-to-Many с Books
        public ICollection<BookGenres> BookGenres { get; set; } = new List<BookGenres>();
    }
}
