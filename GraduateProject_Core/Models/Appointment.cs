using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int StatusID { get; set; }

        public string? Notes { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public string? Status { get; set; }

        public DateTime? RequestedRescheduleDate { get; set; }
        public string? RescheduleReason { get; set; }
        public string? RescheduleStatus { get; set; }
        public int? WaitingMinutes { get; set; }
    }
}
