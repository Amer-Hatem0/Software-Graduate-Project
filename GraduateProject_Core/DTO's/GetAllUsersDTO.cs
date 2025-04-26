using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class GetAllUsersDTO
    {
        public int Id { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public int? age { get; set; }
        public string phone { get; set; }
        public List<string> Roles { get; set; }  
    }
}
