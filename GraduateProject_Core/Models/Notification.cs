using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }

        public Users User { get; set; }


    }
}
