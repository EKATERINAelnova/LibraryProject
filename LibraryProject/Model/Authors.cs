using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{
    [Table("authors")]
    public class Authors
    {
        [Key]
        [Column("author_id")]
        public int AuthorId { get; set; }

        [Column("full_name"), MaxLength(30)]
        public string Name { get; set; } = string.Empty;
    }
}

