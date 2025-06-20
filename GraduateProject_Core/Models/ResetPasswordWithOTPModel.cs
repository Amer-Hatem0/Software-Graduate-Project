using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class ResetPasswordWithOTPModel
    {
        [Key]
        public int id { get; set; }
        public string Username { get; set; }
        public string OTP { get; set; }
        public string NewPassword { get; set; }
    }
}
