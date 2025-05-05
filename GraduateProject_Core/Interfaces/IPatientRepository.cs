using GraduateProject_Core.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface IPatientRepository
    {
        Task<PatientProfileDTO> GetPatientProfileAsync(int patientId);
        Task<IEnumerable<DoctorDTO>> GetAvailableDoctorsAsync();
        Task<bool> BookAppointmentAsync(BookAppointmentDTO dto);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsAsync(int patientId);
        Task<bool> CancelAppointmentAsync(int appointmentId);
        Task<bool> UploadReportAsync(UploadReportDTO dto);
        Task<IEnumerable<PatientReportDTO>> GetReportsAsync(int patientId);
        Task<IEnumerable<MedicalHistoryDTO>> GetMedicalHistoryAsync(int patientId);
    }
}
