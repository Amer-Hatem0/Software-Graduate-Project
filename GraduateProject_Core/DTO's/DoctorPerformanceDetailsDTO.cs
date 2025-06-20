using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class DoctorPerformanceDetailsDTO
    {
        public int TotalPatients { get; set; }
        public int NewPatientsThisMonth { get; set; }
        public double AverageWaitingTimeMinutes { get; set; }
        public int ApprovedLeavesCount { get; set; }
        public int SuccessfulProcedures { get; set; }
    }
}
