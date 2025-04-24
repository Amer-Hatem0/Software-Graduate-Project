using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class DoctorSpecialization
    {

        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }

        public int SpecializationID { get; set; }
        public Specialization Specialization { get; set; }

    }
}
