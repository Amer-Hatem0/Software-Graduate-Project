using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [Authorize(Roles = "Supervisor")]
    public class SupervisorController : ControllerBase
    {


        private readonly AppDbContext _context;

        private readonly ISupervisorRepository _supervisorRepository;

        public SupervisorController(AppDbContext context, ISupervisorRepository supervisorRepository)
        {
            _supervisorRepository = supervisorRepository;
            _context = context;
        }

        [HttpPost("VerifyPatient/{userId}")]
        public async Task<IActionResult> VerifyPatient(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Patient)
                .FirstOrDefaultAsync(u => u.Id == userId && u.Patient != null);

            if (user == null)
                return NotFound(new { Message = "Patient not found." });

            user.IsVerified = true;
            user.Patient.CurrentStatus = "Verified"; // optional

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Patient verified." });
        }


        [HttpGet("Patients")]
        public async Task<IActionResult> GetAllUnverifiedPatients()
        {
            var patients = await _context.Users
                .Include(u => u.Patient)
                .Where(u => u.IsVerified == false && u.Patient != null)
                .Select(u => new
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PhoneNumber
                })
                .ToListAsync();

            return Ok(patients);
        }

        [HttpGet("RescheduleRequests")]
        public async Task<IActionResult> GetRescheduleRequests()
        {
            var requests = await _context.RescheduleRequests
                .Where(r => r.Status == "Pending")
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .Select(r => new
                {
                    r.Id,
                    r.AppointmentId,
                    r.RequestedDateTime,
                    r.Reason,
                    DoctorName = r.Doctor.User.FullName
                })
                .ToListAsync();

            return Ok(requests);
        }

 

        [HttpPost("AssignPatient")]
        public async Task<IActionResult> AssignPatient([FromBody] PatientDoctorAssignmentDTO dto)
        {
            if (dto == null || dto.PatientId == 0 || dto.DoctorId == 0)
                return BadRequest("Invalid input");

            var result = await _supervisorRepository.AssignPatientToDoctorAsync(dto);
            return result ? Ok(new { Message = "Patient assigned to doctor." }) : BadRequest();
        }

        [HttpGet("DoctorLeaveRequests")]
        public async Task<IActionResult> GetDoctorLeaveRequests()
        {
            var requests = await _supervisorRepository.GetAllDoctorLeaveRequestsAsync();
            return Ok(requests);
        }

        [HttpPut("ForwardLeaveRequest/{leaveRequestId}")]
        public async Task<IActionResult> ForwardLeaveRequest(int leaveRequestId)
        {
            var result = await _supervisorRepository.ForwardLeaveRequestToAdminAsync(leaveRequestId);
            return result ? Ok(new { Message = "Leave request forwarded to admin." }) : NotFound();
        }

        [HttpGet("Inventory")]
        public async Task<IActionResult> GetAllInventory()
        {
            var inventory = await _supervisorRepository.GetAllInventoryAsync();
            return Ok(inventory);
        }

        [HttpPost("SubmitInventory")]
        public async Task<IActionResult> SubmitInventoryRequest([FromBody] InventoryRequestDTO dto)
        {
            var result = await _supervisorRepository.SubmitInventoryRequestAsync(dto);
            return result ? Ok(new { Message = "Inventory request submitted." }) : BadRequest();
        }


        [HttpGet("DailyOverview")]
        public async Task<IActionResult> GetDailyOverview()
        {
            var supervisorId = await GetSupervisorIdFromToken();
            var overview = await _supervisorRepository.GetDailyOverviewAsync(supervisorId);
            return Ok(overview);
        }
        [HttpGet("Assignments")]
        public async Task<IActionResult> GetAssignments()
        {
            var result = await _supervisorRepository.GetPatientAssignmentsAsync();
            return Ok(result);
        }
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub || c.Type == "userid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var profile = await _supervisorRepository.GetSupervisorProfileAsync(userId);
            if (profile == null) return NotFound();

            return Ok(profile);
        }
        [HttpGet("TopDoctor")]
        public async Task<IActionResult> GetTopDoctor()
        {
            var topDoctor = await _context.Doctors
                .OrderByDescending(d => d.PerformanceScore)
                .Select(d => new
                {
                    Name = d.User.FullName,
                    Score = d.PerformanceScore
                })
                .FirstOrDefaultAsync();

            if (topDoctor == null)
                return NotFound("No doctor data available");

            return Ok(topDoctor);
        }
        [HttpGet("Patient")]
        public async Task<IActionResult> GetUnverifiedPatients()
        {
            var unverifiedPatients = await _context.Users
                .Include(u => u.Patient)
                .Where(u => u.IsVerified == false && u.Patient != null)
                .Select(u => new
                {
                    FullName = u.FullName,
                    Email = u.Email,
                    IsVerified = u.IsVerified
                })
                .ToListAsync();

            return Ok(unverifiedPatients);
        }



        [HttpGet("Doctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _supervisorRepository.GetAllDoctorsAsync();
            return Ok(doctors);
        }
        [HttpGet("DoctorsPerformance")]
        public async Task<IActionResult> GetDoctorsPerformance()
        {
            var result = await _supervisorRepository.GetDoctorsPerformanceAsync();
            return Ok(result);
        }
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] SupervisorUpdateDTO dto)
        {
            var supervisorId = await GetSupervisorIdFromToken();
            var success = await _supervisorRepository.UpdateSupervisorProfileAsync(supervisorId, dto);
            return success ? Ok(new { message = "Profile updated successfully" }) : BadRequest(new { message = "Update failed" });
        }

        private async Task<int> GetSupervisorIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub ||
                c.Type == "userid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User ID not found in token.");

            if (!int.TryParse(userIdClaim, out int userId))
                throw new Exception($"Invalid user ID format: {userIdClaim}");

            var doctor = await _context.Supervisors
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                throw new Exception("Supervisor not found for this user.");

            return doctor.SupervisorID;
        }


        [HttpPost("RejectReschedule/{requestId}")]
        public async Task<IActionResult> RejectReschedule(int requestId)
        {
            var request = await _context.RescheduleRequests
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == "Pending");

            if (request == null)
                return NotFound("Request not found or already processed.");

            request.Status = "Rejected";

            // تأكد أن الموعد موجود وتم تحميله
            var appointment = request.Appointment;
            if (appointment != null)
            {
                appointment.StatusID = 7; // ⛔️ رفض طلب إعادة الجدولة
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Reschedule request rejected and appointment status updated." });
        }


        [HttpGet("VerifiedPatients")]
        public async Task<IActionResult> GetVerifiedPatients()
        {
            var patients = await _context.Users
                .Include(u => u.Patient)
                .Where(u => u.IsVerified == true && u.Patient != null)
                .Select(u => new
                {
                    PatientId = u.Patient.PatientID,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PhoneNumber
                })
                .ToListAsync();

            return Ok(patients);
        }

        [HttpGet("Appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Select(a => new
                {
                    AppointmentId = a.AppointmentID,
                    PatientName = a.Patient.User.FullName,
                    DoctorName = a.Doctor.User.FullName,
                    AppointmentDate = a.DateTime,
                    StatusID = a.StatusID,
                    Notes = a.Notes
                })
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpPost("ApproveReschedule/{requestId}")]
        public async Task<IActionResult> ApproveReschedule(int requestId)
        {
            var request = await _context.RescheduleRequests
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == "Pending");

            if (request == null)
                return NotFound("Request not found or already processed.");

            request.Status = "Approved";

            var appointment = request.Appointment;
            if (appointment != null)
            {
                appointment.DateTime = request.RequestedDateTime;
                appointment.StatusID = 5; // ✅ Rescheduled
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Reschedule approved and appointment updated." });
        }

    }
}