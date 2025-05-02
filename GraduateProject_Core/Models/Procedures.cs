using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Procedure
    {
        public int ProcedureID { get; set; }
        public int DoctorID { get; set; }
        public string ProcedureType { get; set; }
        public bool Success { get; set; }
        public DateTime DatePerformed { get; set; }
    }

}
