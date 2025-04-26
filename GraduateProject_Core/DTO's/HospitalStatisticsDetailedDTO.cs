using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class HospitalStatisticsDetailedDTO
    {
        public int TotalAppointments { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalSupervisors { get; set; }
        public int TotalFeedbacks { get; set; }
        public List<PatientDetailDTO> Patients { get; set; }
        public List<DoctorDetailDTO> Doctors { get; set; }
        public List<SupervisorDetailDTO> Supervisors { get; set; }
    }

}
