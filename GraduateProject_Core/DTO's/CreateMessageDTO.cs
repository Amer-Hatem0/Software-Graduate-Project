using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class CreateMessageDTO
    {
        public int SenderUserID { get; set; }
        public int ReceiverUserID { get; set; }
        public string Content { get; set; }
    }
}
