using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class LeaveRequestDTO
    {
        public int LeaveRequestId { get; set; }
        public int DoctorId { get; set; }
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; }
    }
}
