using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class AppointmentInfoDTO
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
