using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduateProject_Core.Models
{
    public class RescheduleRequest
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        public DateTime RequestedDateTime { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; } = "Pending"; // Other values: "Approved", "Rejected"

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Appointment Appointment { get; set; }
        public Doctor Doctor { get; set; }
    }
}

