using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("User")]

        public int UserId { get; set; }       
        public DateTime DateOfBirth { get; set; }
 
     
        public string? Gender { get; set; }
        public string CurrentStatus { get; set; }
        public string ComplianceLevel { get; set; }

        public string? Phone { get; set; }
        public AppointmentStatus Status { get; set; }
        public Users User { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<MedicalHistory> MedicalHistories { get; set; }
        public ICollection<AISymptomAnalysis> SymptomAnalyses { get; set; }
        public ICollection<ReportFile> ReportFiles { get; set; }

    }
}
