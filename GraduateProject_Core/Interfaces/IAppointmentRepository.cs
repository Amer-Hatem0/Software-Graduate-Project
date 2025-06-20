using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    
        public interface IAppointmentRepository
        {
            Task<bool> BookAppointmentAsync(BookAppointmentDTO dto);
            Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPatientAsync(int patientId);
            Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
            Task<bool> CancelAppointmentAsync(int appointmentId);
        Task<Appointment> GetAppointmentByIdAsync(int appointmentId);
        Task<bool> CheckIfTimeSlotIsAvailable(int doctorId, DateTime appointmentTime);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date);

    }


}
