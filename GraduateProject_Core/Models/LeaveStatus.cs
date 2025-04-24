using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class LeaveStatus
    {
        [Key]
        public int StatusID { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }

        public ICollection<LeaveRequest> LeaveRequests { get; set; }

    }
}
