using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class LeaveRequestSimpleDTO
    {
        public int RequestID { get; set; }
        public string DoctorName { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
