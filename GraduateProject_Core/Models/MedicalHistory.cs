using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class MedicalHistory
    {
        [Key]
        public int RecordID { get; set; }

      

        [Required]
        public int DoctorID { get; set; }

        [Required]
        public string? Diagnosis { get; set; }

        public string? Treatment { get; set; }

        public DateTime VisitDate { get; set; } = DateTime.UtcNow;
     
        [Required]
        public int PatientID { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
     

        public string? Note { get; set; }     
        public DateTime RecordedAt { get; set; }    
        public bool FollowUpNeeded { get; set; }
    }
}
