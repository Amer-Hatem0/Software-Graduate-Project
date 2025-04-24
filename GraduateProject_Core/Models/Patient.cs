using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Patient
    {
        [Key]
        public int PatientID { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Phone]
        public string Phone { get; set; }

        public Users User { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<MedicalHistory> MedicalHistories { get; set; }
        public ICollection<AISymptomAnalysis> SymptomAnalyses { get; set; }
        public ICollection<ReportFile> ReportFiles { get; set; }

    }
}
