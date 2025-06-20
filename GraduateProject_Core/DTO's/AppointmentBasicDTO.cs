using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class AppointmentBasicDTO
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string StatusName { get; set; }
        public string PatientName { get; set; }
        public int StatusID { get; set; }
    }

}
