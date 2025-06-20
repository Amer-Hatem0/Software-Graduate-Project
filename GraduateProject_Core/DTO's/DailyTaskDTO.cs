using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class DailyTaskDTO
    {
        public int AppointmentsToday { get; set; }
        public int PendingReschedules { get; set; }
        public int PatientsNeedingFollowUp { get; set; }
    }
}
