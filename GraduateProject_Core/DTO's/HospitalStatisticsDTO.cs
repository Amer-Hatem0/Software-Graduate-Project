using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class HospitalStatisticsDTO
    {
        public int TotalAppointments { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalSupervisors { get; set; }
        public int TotalFeedbacks { get; set; }
    }
}
