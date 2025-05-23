﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Specialization
    {
        [Key]
        public int SpecializationID { get; set; }
        public string Name { get; set; }

        public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; }

    }
}
