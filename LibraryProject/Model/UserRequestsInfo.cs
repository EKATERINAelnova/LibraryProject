using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Model
{
    public class UserRequestsInfo
    {
        public int Request_Id { get; set; }
        public int Copy_Id { get; set; }
        public string Book_Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Request_Date { get; set; }
        public DateTime Delete_Date { get; set; }
        public int Days_Left { get; set; }
    }

}
