using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Core.DTO_s;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GraduateProject_Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

         public async Task<IEnumerable<GetAllUsersDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new GetAllUsersDTO
                {
                    fullName = u.FullName,
                    email = u.Email,
                    gender = u.Gender,
                    age = u.Age,
                    phone = u.PhoneNumber,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id,   (ur, r) => r.Name)
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<GetAllUsersDTO> GetUserByIdAsync(int userId)
        {

          var u=   await _context.Users.FindAsync(userId);

            var use = new GetAllUsersDTO
            {
                Id = userId,
                fullName = u.FullName,
                email = u.Email,
                gender = u.Gender,
                age = u.Age,
                phone = u.PhoneNumber,
                Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList()

            };
            return  use ;


        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

             var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            if (userRoles.Any())
            {
                _context.UserRoles.RemoveRange(userRoles);
            }

             _context.Users.Remove(user);

             await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                                 .Include(l => l.Doctor)
                                 .ToListAsync();
        }

        public async Task<bool> ApproveLeaveRequestAsync(int leaveRequestId)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
            if (leaveRequest == null)
                return false;

            leaveRequest.StatusID = 1;  

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectLeaveRequestAsync(int leaveRequestId)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
            if (leaveRequest == null)
                return false;

            leaveRequest.StatusID = 2;  

            await _context.SaveChangesAsync();
            return true;
        }

         public async Task<IEnumerable<Inventory>> GetAllInventoryRequestsAsync()
        {
            return await _context.Inventory.ToListAsync();
        }

        public async Task<bool> ApproveInventoryRequestAsync(int inventoryId)
        {
            var inventoryItem = await _context.Inventory.FindAsync(inventoryId);
            if (inventoryItem == null)
                return false;

            inventoryItem.Status = "Approved";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectInventoryRequestAsync(int inventoryId)
        {
            var inventoryItem = await _context.Inventory.FindAsync(inventoryId);
            if (inventoryItem == null)
                return false;

            inventoryItem.Status = "Rejected";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditInventoryRequestAsync(int inventoryId, InventoryUpdateDTO updateDto)
        {
            var inventoryItem = await _context.Inventory.FindAsync(inventoryId);
            if (inventoryItem == null)
                return false;

            inventoryItem.Quantity = updateDto.Quantity;
            inventoryItem.Notes = updateDto.Notes;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<HospitalStatisticsDetailedDTO> GetHospitalStatisticsAsync()
        {
            var totalAppointments = await _context.Appointments.CountAsync();
            var totalPatients = await _context.Patients.CountAsync();
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalSupervisors = await _context.Supervisors.CountAsync();
            var feedbackCount = await _context.Feedbacks.CountAsync();

               var patientDetails = await _context.Patients
                .Include(p => p.User)
                .Select(p => new PatientDetailDTO
                {
                    PatientId = p.PatientID,
                    FullName = p.User.FullName,
                    Email = p.User.Email,
                    Phone = p.User.PhoneNumber,
                    Age = p.User.Age,
                    Gender = p.User.Gender
                })
                .ToListAsync();

         var doctorDetails = await _context.Doctors
                .Include(d => d.User)
                .Select(d => new DoctorDetailDTO
                {
                    DoctorId = d.DoctorID,
                    FullName = d.User.FullName,
                    Email = d.User.Email,
                    Phone = d.User.PhoneNumber,
                    Specialization = d.User.Specialization,
                    PerformanceScore = d.PerformanceScore
                })
                .ToListAsync();

          
            var supervisorDetails = await _context.Supervisors
                .Include(s => s.User)
                .Select(s => new SupervisorDetailDTO
                {
                    SupervisorId = s.SupervisorID,
                    FullName = s.User.FullName,
                    Email = s.User.Email,
                    Phone = s.User.PhoneNumber
                })
                .ToListAsync();

            return new HospitalStatisticsDetailedDTO
            {
                TotalAppointments = totalAppointments,
                TotalPatients = totalPatients,
                TotalDoctors = totalDoctors,
                TotalSupervisors = totalSupervisors,
                TotalFeedbacks = feedbackCount,
                Patients = patientDetails,
                Doctors = doctorDetails,
                Supervisors = supervisorDetails
            };
        }

    }
}
