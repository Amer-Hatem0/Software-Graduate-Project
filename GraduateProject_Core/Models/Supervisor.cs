using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Supervisor
    {
        [Key]
        public int SupervisorID { get; set; }
        public int UserId { get; set; }
        public string? Department { get; set; }

        public Users User { get; set; }

    }
}
