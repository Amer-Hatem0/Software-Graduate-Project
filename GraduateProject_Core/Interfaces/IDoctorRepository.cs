using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface IDoctorRepository
    {
        Task<bool> RequestLeaveAsync(LeaveRequestDTO leaveRequest);
    }
}
