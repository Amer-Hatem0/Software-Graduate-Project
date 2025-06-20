using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class DoctorPerformanceDTO
    {
        public int TotalAppointments { get; set; }
        public double AverageRating { get; set; }
        public string DoctorName { get; set; }
        public double PerformanceScore { get; set; }
        public int PatientCount { get; set; }
        public string Workload { get; set; }
    }
}
