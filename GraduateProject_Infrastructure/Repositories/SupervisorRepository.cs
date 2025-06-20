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
            var patient = await _context.Patients
                .Include(p => p.User)  
                .FirstOrDefaultAsync(p => p.PatientID == patientId);

            if (patient == null || patient.User == null)
                return false;
            patient.User.IsVerified = true;
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
        public async Task<IEnumerable<DoctorPerformanceDTO>> GetDoctorsPerformanceAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Select(d => new DoctorPerformanceDTO
                {
                    DoctorName = d.User.FullName,

                     
                    PerformanceScore = Math.Round(
                        _context.Feedbacks
                            .Where(f => f.DoctorID == d.DoctorID)
                            .Select(f => (double?)f.Rating)
                            .Average() ?? 0, 2),

                    PatientCount = _context.Appointments
                        .Where(a => a.DoctorID == d.DoctorID)
                        .Select(a => a.PatientID)
                        .Distinct()
                        .Count(),

                    Workload = "Moderate"
                })
                .ToListAsync();
        }

        public async Task<SupervisorProfileDTO> GetSupervisorProfileAsync(int userId)
        {
            var supervisor = await _context.Supervisors
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (supervisor == null) return null;

            return new SupervisorProfileDTO
            {
                FullName = supervisor.User.FullName,
                Email = supervisor.User.Email,
                PhoneNumber = supervisor.User.PhoneNumber,
                Gender = supervisor.User.Gender,
                DateOfBirth = (DateTime)supervisor.User.DateOfBirth
            };
        }
        public async Task<bool> UpdateSupervisorProfileAsync(int supervisorId, SupervisorUpdateDTO dto)
        {
            var supervisor = await _context.Supervisors.Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SupervisorID == supervisorId);

            if (supervisor == null) return false;

            supervisor.User.FullName = dto.FullName;
            supervisor.User.Email = dto.Email;
            supervisor.User.PhoneNumber = dto.PhoneNumber;
            supervisor.User.Gender = dto.Gender;
            supervisor.User.DateOfBirth = dto.DateOfBirth;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignPatientToDoctorAsync(PatientDoctorAssignmentDTO dto)
        {
            var patient = await _context.Patients.FindAsync(dto.PatientId);
            if (patient == null) return false;

            var appointment = new Appointment
            {
                PatientID = dto.PatientId,
                DoctorID = dto.DoctorId,
                DateTime = dto.DateTime,
                StatusID = 1,
                Notes = "Booked from supervisor panel"
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
                    DoctorName = l.Doctor.User.FullName,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Reason = l.Reason,
                    SubmittedAt = l.SubmittedAt ?? DateTime.MinValue,
                    Status = l.Status.StatusName
                })
                .ToListAsync();
        }
        public async Task<List<DoctorDTO>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)  
                .Select(d => new DoctorDTO
                {
                    DoctorId = d.DoctorID,
                    FullName = d.User.FullName,
                    Email = d.User.Email,
                    Gender = d.User.Gender,
                    Specialization = d.Specialization,
                    ProfileImage = d.User.ProfileImage

                })
                .ToListAsync();

            return doctors;
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
        public async Task<List<PatientAssignmentDTO>> GetPatientAssignmentsAsync()
        {
            var assignments = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .OrderByDescending(a => a.DateTime)
                .Select(a => new PatientAssignmentDTO
                {
                    PatientName = a.Patient.User.FullName,
                    DoctorName = a.Doctor.User.FullName,
                    AssignedAt = a.DateTime
                })
                .ToListAsync();

            return assignments;
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
