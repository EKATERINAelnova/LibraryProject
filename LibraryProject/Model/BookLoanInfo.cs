using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{
    [Table("all_book_loans_info")]
    public class BookLoanInfo
    {
        [Key]
        [Column("loan_id")]
        public int LoanId { get; set; }

        [Column("copy_id")]
        public int CopyId { get; set; }

        [Column("book_id")]
        public int BookId { get; set; }

        [Column("request_id")]
        public int RequestId { get; set; }

        [Column("book_title")]
        public string? BookTitle { get; set; }

        [Column("authors")]
        public string? Authors { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("reader_name")]
        public string? Login { get; set; }

        [Column("loan_date")]
        public DateTime LoanDate { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("return_date")]
        public DateTime? ReturnDate { get; set; }

        [Column("status")]
        public string? Status { get; set; }
    }
}
