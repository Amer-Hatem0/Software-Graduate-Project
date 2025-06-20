using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class VisitDTO
    {
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
    }
}
