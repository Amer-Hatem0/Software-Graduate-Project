using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class PatientAssignmentDTO
    {
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
