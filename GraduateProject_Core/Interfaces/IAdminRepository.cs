using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;

namespace GraduateProject_Core.Interfaces
{
    public interface IAdminRepository
    {



        Task<IEnumerable<GetAllUsersDTO>> GetAllUsersAsync();
        Task<GetAllUsersDTO> GetUserByIdAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);

        //  Managing leave requests
        Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync();
        Task<bool> ApproveLeaveRequestAsync(int leaveRequestId);
        Task<bool> RejectLeaveRequestAsync(int leaveRequestId);

        // Medical inventory request management
        Task<IEnumerable<Inventory>> GetAllInventoryRequestsAsync();
        Task<bool> ApproveInventoryRequestAsync(int inventoryId);
        Task<bool> RejectInventoryRequestAsync(int inventoryId);
        Task<bool> EditInventoryRequestAsync(int inventoryId, InventoryUpdateDTO updateDto);

        // View performance reports
        Task<HospitalStatisticsDetailedDTO> GetHospitalStatisticsAsync();


    }
}
