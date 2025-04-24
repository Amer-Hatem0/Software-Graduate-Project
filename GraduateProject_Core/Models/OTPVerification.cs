using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class OTPVerification
    {
        [Key]
        public int OTPID { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(10)]
        public string Type { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public Users User { get; set; }


    }
}
