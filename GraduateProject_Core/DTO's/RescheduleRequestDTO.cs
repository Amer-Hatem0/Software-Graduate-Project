using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class RescheduleRequestDTO
    {
        public int AppointmentId { get; set; }
        public DateTime NewDate { get; set; }
        public string Reason { get; set; }
        public int DoctorId { get; set; }
    }
}
