using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class FeedbackDisplayDTO
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public int Rating { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
