using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Doctor 
    {
        [Key]
        public int DoctorID { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; }

        public float PerformanceScore { get; set; }
        public int Workload { get; set; }

        public Users User { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<MedicalHistory> MedicalHistories { get; set; }
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
        public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; }

    }
}
