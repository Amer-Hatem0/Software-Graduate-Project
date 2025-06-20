using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Core.DTO_s;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace GraduateProject_Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;


        public AdminRepository(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<GetAllUsersDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new GetAllUsersDTO
                {
                    Id = u.Id,
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
            var u = await _context.Users.FindAsync(userId);
            if (u == null) return null;

            var use = new GetAllUsersDTO
            {
                Id = userId,
                fullName = u.FullName,
                email = u.Email,
                gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                age = u.Age,
                phone = u.PhoneNumber,
                ProfileImage = u.ProfileImage,  
                Roles = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToList()
            };
            return use;
        }
        
        public async Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Select(a => new AppointmentDTO
                {
                    AppointmentId = a.AppointmentID,
                    PatientID = a.PatientID,
                    PatientName = a.Patient.User.FullName,
                    DoctorID = a.DoctorID,
                    DoctorName = a.Doctor.User.FullName,
                    DoctorUserID = a.Doctor.UserId,
                    AppointmentDate = a.DateTime,
                    StatusID = a.StatusID,
                    Notes = a.Notes
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            var messages = await _context.Messages
                .Where(m => m.SenderUserID == userId || m.ReceiverUserID == userId)
                .ToListAsync();
            _context.Messages.RemoveRange(messages);

             var otps = await _context.OTPVerifications
                .Where(o => o.UserId == userId)
                .ToListAsync();
            _context.OTPVerifications.RemoveRange(otps);

             var patient = await _context.Patients
                .Include(p => p.Appointments)
                .Include(p => p.Feedbacks)
                .Include(p => p.MedicalHistories)
                .Include(p => p.SymptomAnalyses)
                .Include(p => p.ReportFiles)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient != null)
            {
                _context.Appointments.RemoveRange(patient.Appointments);
                _context.Feedbacks.RemoveRange(patient.Feedbacks);
                _context.MedicalHistories.RemoveRange(patient.MedicalHistories);
                _context.AISymptomAnalyses.RemoveRange(patient.SymptomAnalyses);
                _context.ReportFiles.RemoveRange(patient.ReportFiles);
                _context.Patients.Remove(patient);
            }

            var doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .Include(d => d.LeaveRequests)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor != null)
            {
                _context.Appointments.RemoveRange(doctor.Appointments);
                _context.LeaveRequests.RemoveRange(doctor.LeaveRequests);
                _context.Doctors.Remove(doctor);
            }

            var supervisor = await _context.Supervisors
                .FirstOrDefaultAsync(s => s.UserId == userId);
            if (supervisor != null)
            {
                _context.Supervisors.Remove(supervisor);
            }

             var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
            _context.UserRoles.RemoveRange(userRoles);

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

        //public async Task<bool> ApproveLeaveRequestAsync(int leaveRequestId)
        //{
        //    var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
        //    if (leaveRequest == null)
        //        return false;

        //    leaveRequest.StatusID = 1;  

        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> ApproveLeaveRequestAsync(int leaveRequestId)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(l => l.RequestID == leaveRequestId);

            if (leaveRequest == null)
                return false;

            // تعيين الحالة "Approved"
            leaveRequest.StatusID = (await _context.LeaveStatuses.FirstOrDefaultAsync(s => s.StatusName == "Approved"))?.StatusID ?? 1;

            // جلب كل المواعيد التي ضمن فترة الإجازة والتي لم تُلغ بعد
            var canceledStatusId = (await _context.AppointmentStatuses.FirstOrDefaultAsync(s => s.StatusName == "Canceled"))?.StatusID ?? 4;

            var affectedAppointments = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a =>
                    a.DoctorID == leaveRequest.DoctorID &&
                    a.DateTime.Date >= leaveRequest.StartDate.Date &&
                    a.DateTime.Date <= leaveRequest.EndDate.Date &&
                    a.StatusID != canceledStatusId)
                .ToListAsync();

            foreach (var appointment in affectedAppointments)
            {
                appointment.StatusID = canceledStatusId;

                _context.Notifications.Add(new Notification
                {
                    UserId = appointment.Patient.UserId,
                    Title = "Appointment Canceled",
                    Message = $"Dear {appointment.Patient.User.FullName}, your appointment on {appointment.DateTime:MMM dd, yyyy HH:mm} with Dr. {leaveRequest.Doctor.User.FullName} has been canceled due to doctor's approved leave. Please book another appointment.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }

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
      
        public async Task<bool> UpdateUserAsync(int userId, AdminUpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Gender = dto.Gender;
            user.Age = dto.Age;
            user.UserName = dto.Email;

         
            if (!string.IsNullOrEmpty(dto.ProfileImage))
            {
                user.ProfileImage = dto.ProfileImage;
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<ReportFDTO>> GetAllReportFilesAsync()
        {
            return await _context.ReportFiles
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Select(r => new ReportFDTO
                {
                    Id = r.ReportID,
                    FileName = r.FileName,
                    FileUrl = r.FileUrl,
                    UploadedAt = r.UploadedAt,
                    PatientName = r.Patient.User.FullName
                })
                .ToListAsync();
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
