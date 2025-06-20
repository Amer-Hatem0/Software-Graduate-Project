using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface IDoctorRepository
    {
        Task<bool> RequestLeaveAsync(LeaveRequestDTO leaveRequest);
        Task<bool> RequestAppointmentRescheduleAsync(RescheduleRequestDTO dto);
        Task<bool> AddPatientNoteAsync(PatientNoteDTO dto, int doctorId);
        Task<bool> EvaluatePatientComplianceAsync(PatientComplianceDTO dto);
        Task<IEnumerable<AppointmentDTO>> GetWorkHistoryAsync(int doctorId);
        Task<DailyTaskDetailsDTO> GetDailyTasksAsync(int doctorId);
        Task<DoctorPerformanceDetailsDTO> GetEnhancedPerformanceReportAsync(int doctorId);
        Task<bool> UploadPatientReportAsync(UploadReportDTO dto);
        Task<PatientFullHistoryDTO> GetPatientFullHistoryAsync(int patientId);
        byte[] GeneratePatientHistoryPdf(PatientFullHistoryDTO history);
        Task<PatientFullHistoryDTO> GetFullPatientHistoryAsync(int patientId);
        Task<List<PatientBasicInfoDTO>> GetMyPatientsAsync(int doctorId);
        Task<IEnumerable<AppointmentBasicDTO>> GetAppointmentsForDoctorAsync(int doctorId);
        Task<List<LeaveRequestDTO>> GetLeaveRequestsAsync(int doctorId);
        Task<bool> UploadReportFileAsync(UploadReportFileDTO dto);
        Task<List<ReportFileDTO>> GetPatientReportsAsync(int patientId);
        Task<int> GetUserIdByDoctorIdAsync(int doctorId);
        Task<IEnumerable<UserSimpleDTO>> GetMyChatPatientsAsync(int doctorId, int doctorUserId);
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByDoctorAsync(int doctorId);


    }
}
