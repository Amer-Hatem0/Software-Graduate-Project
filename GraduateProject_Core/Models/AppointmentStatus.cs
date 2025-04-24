using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class AppointmentStatus
    {
        [Key]
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

    }
}
