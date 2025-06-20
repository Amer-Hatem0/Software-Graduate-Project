using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class BookAppointmentDTO
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime DateTime { get; set; }
        public int DoctorUserID { get; set; }
        public int StatusID { get; set; }

        public string? Status { get; set; }
        public string? Notes { get; set; }
        
    }
}
