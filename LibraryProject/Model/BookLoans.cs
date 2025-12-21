using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{
    [Table("book_loans")]
    public class BookLoans
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("reader_id")]
        public int ReaderId { get; set; }

        [Column("copy_id")]
        public int CopyBookId { get; set; }

        [Column("worker_id")]
        public int WorkerId { get; set; }

        [Column("loan_date")]
        public DateTime LoanDate { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("return_date")]
        public DateTime? ReturnDate { get; set; }

        [Column("status"), MaxLength(20)]
        public string Status { get; set; } = "выдана";
        [ForeignKey("ReaderId")]
        public Readers? Reader { get; set; }

        [ForeignKey("WorkerId")]
        public Workers? Worker { get; set; }
        [ForeignKey("CopyId")]
        public CopyBooks? CopyBook { get; set; }
    }
}
