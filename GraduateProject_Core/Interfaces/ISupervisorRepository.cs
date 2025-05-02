using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface ISupervisorRepository
    {
        Task<bool> VerifyNewPatientAsync(int patientId);
        Task<IEnumerable<PatientDTO>> GetAllPatientsAsync();
        Task<bool> AssignPatientToDoctorAsync(PatientDoctorAssignmentDTO dto);

        Task<IEnumerable<LeaveRequestDTO>> GetAllDoctorLeaveRequestsAsync();
        Task<bool> ForwardLeaveRequestToAdminAsync(int leaveRequestId);

        Task<IEnumerable<Inventory>> GetAllInventoryAsync();
        Task<bool> SubmitInventoryRequestAsync(InventoryRequestDTO dto);

        Task<SupervisorDailyOverviewDTO> GetDailyOverviewAsync(int supervisorId);
    }
}
