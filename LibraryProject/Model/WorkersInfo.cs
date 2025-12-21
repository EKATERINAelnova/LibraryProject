using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    public class WorkersInfo
    {
        [Key]
        public int UserId { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "reader";

        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Inn { get; set; }

        public string? Gender { get; set; }

        public string Passport { get; set; } = string.Empty;

        public string? Position { get; set; } = "библиотекарь";


        public int? Salary { get; set; }


        public string? Address { get; set; }


        public string? PhoneNumber { get; set; }
    }
}
