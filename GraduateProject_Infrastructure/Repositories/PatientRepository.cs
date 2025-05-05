using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PatientProfileDTO> GetPatientProfileAsync(int patientId)
        {
            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.PatientID == patientId);
            if (patient == null) return null;

            return new PatientProfileDTO
            {
                PatientId = patient.PatientID,
                FullName = patient.User.FullName,
                Email = patient.User.Email,
                Phone = patient.Phone,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                ComplianceLevel = patient.ComplianceLevel,
                CurrentStatus = patient.CurrentStatus
            };
        }

        public async Task<IEnumerable<DoctorDTO>> GetAvailableDoctorsAsync()
        {
            return await _context.Doctors.Include(d => d.User).Select(d => new DoctorDTO
            {
                DoctorId = d.DoctorID,
                FullName = d.User.FullName,
                Email = d.User.Email,
                Specialization = d.Specialization
            }).ToListAsync();
        }

        public async Task<bool> BookAppointmentAsync(BookAppointmentDTO dto)
        {
            var appointment = new Appointment
            {
                PatientID = dto.PatientId,
                DoctorID = dto.DoctorId,
                DateTime = dto.DateTime,
                StatusID = 1
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.PatientID == patientId)
                .Select(a => new AppointmentDTO
                {
                    AppointmentId = a.AppointmentID,
                    DoctorName = a.Doctor.User.FullName,
                    AppointmentDate = a.DateTime,
                    StatusName = a.Status
                }).ToListAsync();
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return false;
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UploadReportAsync(UploadReportDTO dto)
        {
            if (dto.ReportFile != null && dto.ReportFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await dto.ReportFile.CopyToAsync(memoryStream);

                var report = new PatientReport
                {
                    PatientID = dto.PatientId,
                    FileName = dto.ReportFile.FileName,
                    ContentType = dto.ReportFile.ContentType,
                    Data = memoryStream.ToArray(),
                    UploadedAt = DateTime.UtcNow,
                    Description = dto.Description
                };

                await _context.PatientReports.AddAsync(report);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<PatientReportDTO>> GetReportsAsync(int patientId)
        {
            return await _context.PatientReports
                .Where(r => r.PatientID == patientId)
                .Select(r => new PatientReportDTO
                {
                    ReportId = r.ReportID,
                    FileName = r.FileName,
                    UploadedAt = r.UploadedAt,
                    Description = r.Description
                }).ToListAsync();
        }

        public async Task<IEnumerable<MedicalHistoryDTO>> GetMedicalHistoryAsync(int patientId)
        {
            return await _context.MedicalHistories
                .Where(m => m.PatientID == patientId)
                .Select(m => new MedicalHistoryDTO
                {
                    Diagnosis = m.Diagnosis,
                    Treatment = m.Treatment,
                    VisitDate = m.VisitDate,
                    Note = m.Note,
                    DoctorName = m.Doctor.User.FullName
                }).ToListAsync();
        }
    }
}

