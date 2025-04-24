using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class LeaveRequest
    {
        [Key]
        public int RequestID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [Required]
        public int StatusID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(300)]
        public string Reason { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public Doctor Doctor { get; set; }
        public LeaveStatus Status { get; set; }


    }
}
