using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class RescheduleRequestInfoDTO
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public DateTime RequestedNewDate { get; set; }
        public string Reason { get; set; }
    }
}
