using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class AppointmentDTO
    {
 
        public int AppointmentId { get; set; }
        public string? PatientName { get; set; }
        public int PatientID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? StatusName{ get; set; }
        public string? DoctorName { get; set; }
        public int DoctorID { get; set; }
        public string? Notes { get; set; }
        public int DoctorUserID { get; set; }
        public int StatusID { get; set; }
    }
}
