using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class AISymptomTemplate
    {
        [Key]
        public int ID { get; set; }
        public string Category { get; set; }
        public string CommonSymptoms { get; set; }

    }
}
