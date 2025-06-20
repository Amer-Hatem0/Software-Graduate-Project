using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class SupervisorDailyOverviewDTO
    {
        public int TotalPatients { get; set; }
        public int PendingInventoryRequests { get; set; }
        public int PendingLeaveRequests { get; set; }
    }
}
