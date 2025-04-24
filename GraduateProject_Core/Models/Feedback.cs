using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }


    }
}
