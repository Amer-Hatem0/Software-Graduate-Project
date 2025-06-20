using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class PatientComplianceDTO
    {
        public int PatientId { get; set; }
        public string ComplianceLevel { get; set; } // مثلا: "Excellent", "Good", "Poor"
    }
}
