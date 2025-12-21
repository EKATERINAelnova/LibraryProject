using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryProject.Model
{
    public class StatWorkers
    {
        [Column ("full_name")]
        public string FullName { get; set; } = string.Empty;
        [Column("login")]
        public string Login { get; set; } = string.Empty;
        [Column("loans_count")]
        public int LoansCount { get; set; }

        [Column("unique_workers_count")]
        public int UniqueWorkersCount { get; set; }
    }

}
