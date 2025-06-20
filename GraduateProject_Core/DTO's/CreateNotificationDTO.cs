using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class CreateNotificationDTO
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
