using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    [Table("copy_books")]
    public class CopyBooks
    {
        [Key]
        [Column("id")]
        public int CopyId { get; set; }

        [Column("section_id")]
        public int SectionId { get; set; }

        [Column("closet")]
        public int Closet { get; set; }

        [Column("shelf")]
        public int Shelf { get; set; }

        [Column("place")]
        public int Place { get; set; }

        [Column("year_publish")]
        public int? YearPublish { get; set; }

        [Column("num_of_pages")]
        public int NumOfPages { get; set; }

        [Column("library_cipher")]
        public string? LibraryCipher { get; set; }

        [Column("book_id")]
        public int BookId { get; set; }

        [Column("publisher_id")]
        public int PublisherId { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [ForeignKey("BookId")]
        public Books? Book { get; set; }

        [ForeignKey("StatusId")]
        public CopyStatus? Status { get; set; }

        [ForeignKey("SectionId")]
        public Sections? Section { get; set; }

        [ForeignKey("PublisherId")]
        public Publishers? Publisher { get; set; }
    }
}
