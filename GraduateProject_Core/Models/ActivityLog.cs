using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class ActivityLog
    {
        [Key]
        public int LogID { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TableAffected { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }

        public Users User { get; set; }

    }
}
