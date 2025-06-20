using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
{
    return await _context.Appointments.FindAsync(appointmentId);
}

        public async Task<bool> BookAppointmentAsync(BookAppointmentDTO dto)
        {
            try
            {
                var appointment = new Appointment
                {
                    DoctorID = dto.DoctorId,
                    PatientID = dto.PatientId,
                    DateTime = DateTime.SpecifyKind(dto.DateTime, DateTimeKind.Local),
                    StatusID = dto.StatusID,
                    Notes = dto.Notes
                };


                _context.Appointments.Add(appointment);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                throw;
            }


             
        }
        public async Task<bool> CheckIfTimeSlotIsAvailable(int doctorId, DateTime appointmentTime)
        {
            var conflictingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.DoctorID == doctorId &&
                                          a.DateTime == appointmentTime);

            return conflictingAppointment == null;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorID == doctorId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            return await _context.Appointments
                .Where(a => a.DoctorID == doctorId && a.DateTime >= startOfDay && a.DateTime < endOfDay)
                .ToListAsync();
        }

 

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPatientAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.AppointmentStatus)  
                .Where(a => a.PatientID == patientId)
                .Select(a => new AppointmentDTO
                {
                    AppointmentId = a.AppointmentID,
                    DoctorName = a.Doctor.User.FullName,
                    DoctorID = a.DoctorID,
                    PatientID = a.PatientID,
                    DoctorUserID = a.Doctor.User.Id,
                    AppointmentDate = a.DateTime,
                    StatusID = a.StatusID,
                    StatusName = a.AppointmentStatus.StatusName,  
                    Notes = a.Notes
                }).ToListAsync();
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return false;

            _context.Appointments.Remove(appointment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
