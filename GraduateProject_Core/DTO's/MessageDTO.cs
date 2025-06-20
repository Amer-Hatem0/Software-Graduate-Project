using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class MessageDTO
    {
        public int MessageID { get; set; }
        public int SenderUserID { get; set; }
        public int ReceiverUserID { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
