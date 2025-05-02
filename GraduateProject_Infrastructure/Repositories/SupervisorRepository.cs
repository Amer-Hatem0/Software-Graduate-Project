using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace GraduateProject_Infrastructure.Repositories
{
    public class SupervisorRepository : ISupervisorRepository
    {
        private readonly AppDbContext _context;

        public SupervisorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> VerifyNewPatientAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return false;

            patient.CurrentStatus = "Verified";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PatientDTO>> GetAllPatientsAsync()
        {
            return await _context.Patients
                .Include(p => p.User)
                .Select(p => new PatientDTO
                {
                    PatientId = p.PatientID,
                    FullName = p.User.FullName,
                    Email = p.User.Email,
                    Phone = p.Phone,
                    Status = p.CurrentStatus
                })
                .ToListAsync();
        }

        public async Task<bool> AssignPatientToDoctorAsync(PatientDoctorAssignmentDTO dto)
        {
            var patient = await _context.Patients.FindAsync(dto.PatientId);
            if (patient == null) return false;

            var appointment = new Appointment
            {
                PatientID = dto.PatientId,
                DoctorID = dto.DoctorId,
                DateTime = DateTime.Now.AddDays(1),
                StatusID = 1 // Scheduled
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LeaveRequestDTO>> GetAllDoctorLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(l => l.Doctor)
                .Include(l => l.Status)
                .Select(l => new LeaveRequestDTO
                {
                    
                    LeaveRequestId = l.RequestID,
                    DoctorId = l.DoctorID,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Reason = l.Reason,
                    SubmittedAt = l.SubmittedAt ?? DateTime.MinValue,
                    Status = l.Status.StatusName
                })
                .ToListAsync();
        }

        public async Task<bool> ForwardLeaveRequestToAdminAsync(int leaveRequestId)
        {
            var request = await _context.LeaveRequests.FindAsync(leaveRequestId);
            if (request == null) return false;

            request.StatusID = 5; // ForwardedToAdmin
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Inventory>> GetAllInventoryAsync()
        {
            return await _context.Inventory.ToListAsync();
        }

        public async Task<bool> SubmitInventoryRequestAsync(InventoryRequestDTO dto)
        {
            var item = new Inventory
            {
                ItemName = dto.Name,
                Quantity = dto.Quantity,
                Notes = dto.Notes,
                Status = "Pending"
            };
            _context.Inventory.Add(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SupervisorDailyOverviewDTO> GetDailyOverviewAsync(int supervisorId)
        {
            var patientCount = await _context.Patients.CountAsync();
            var pendingInventory = await _context.Inventory.CountAsync(i => i.Status == "Pending");
            var pendingLeaves = await _context.LeaveRequests.CountAsync(l => l.StatusID == 3); // pending

            return new SupervisorDailyOverviewDTO
            {
                TotalPatients = patientCount,
                PendingInventoryRequests = pendingInventory,
                PendingLeaveRequests = pendingLeaves
            };
        }
    }
}
