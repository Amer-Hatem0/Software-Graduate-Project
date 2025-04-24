using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Users : IdentityUser<int>
    {


 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Admin Admin { get; set; }
            public Supervisor Supervisor { get; set; }
            public Patient Patient { get; set; }
            public Doctor Doctor { get; set; }


            public ICollection<ActivityLog> ActivityLogs { get; set; }
            public ICollection<OTPVerification> OTPVerifications { get; set; }
            public ICollection<Notification> Notifications { get; set; }
      
        }
}
