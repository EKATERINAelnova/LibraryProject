using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{

    public class StatReaders
    {
        [Column ("category")]
        public string Category { get; set; } = string.Empty;
        [Column ("count")]
        public int Count { get; set; }
    }
}

