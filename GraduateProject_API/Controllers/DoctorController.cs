using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using GraduateProject_Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  [Authorize(Roles = "Doctor,Patient")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<DoctorController> _logger;
        private readonly IChatRepository _chatRepository;
        private readonly INotificationRepository _notificationRepository;

        public DoctorController(IDoctorRepository doctorRepository, AppDbContext context, INotificationRepository notificationRepository , ILogger<DoctorController> logger, IChatRepository chatRepository)
        {
            _doctorRepository = doctorRepository;
            _context = context;
            _logger = logger;
            _chatRepository = chatRepository; 
            _notificationRepository = notificationRepository;
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpGet("LeaveRequests/{doctorId}")]
        public async Task<IActionResult> GetLeaveRequestsByDoctor(int doctorId)
        {
            var leaveRequests = await _doctorRepository.GetLeaveRequestsByDoctorAsync(doctorId);
            return Ok(leaveRequests);
        }




        [HttpDelete("DeleteLeaveRequest/{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            _context.LeaveRequests.Remove(request);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted successfully" });
        }

        //  1. طلب إجازة
        [HttpPost("RequestLeave")]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveRequestDTO dto)
        {
            var doctorId = await GetDoctorIdFromToken();
            dto.DoctorId = doctorId;

             _logger.LogInformation("📥 Leave request received: DoctorId={DoctorId}, Start={Start}, End={End}, Reason={Reason}",
                dto.DoctorId, dto.StartDate, dto.EndDate, dto.Reason);

            var result = await _doctorRepository.RequestLeaveAsync(dto);
            return result ? Ok(new { Message = "Leave request submitted." }) : BadRequest(new { Message = "❌ RequestLeaveAsync returned false." });
        }

        [HttpGet("MyLeaveRequests")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            var doctorId = await GetDoctorIdFromToken();
            var data = await _doctorRepository.GetLeaveRequestsAsync(doctorId);
            return Ok(data);
        }

        ////  2. طلب إعادة جدولة موعد
        //[HttpPost("RequestReschedule")]
        //public async Task<IActionResult> RequestAppointmentReschedule([FromBody] RescheduleRequestDTO dto)
        //{
        //    var result = await _doctorRepository.RequestAppointmentRescheduleAsync(dto);
        //    if (result)
        //    {
        //        var notification = new Notification
        //        {
        //            UserId = dto.PatientUserId,
        //            Title = "Reschedule Request 🔄",
        //            Message = "Doctor requested to reschedule your appointment.",
        //            CreatedAt = DateTime.UtcNow,
        //            IsRead = false
        //        };
        //        await _notificationRepository.CreateNotificationAsync(notification);
        //        return Ok(new { Message = "Reschedule request submitted." });
        //    }

        //    return BadRequest();
        //}


        [HttpPost("AddPatientNote")]
        public async Task<IActionResult> AddPatientNote([FromBody] PatientNoteDTO dto)
        {
            try
            {
                var doctorId = await GetDoctorIdFromToken();   
                var result = await _doctorRepository.AddPatientNoteAsync(dto, doctorId);

                return result ? Ok(new { Message = "Note added to patient history." }) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding patient note.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("MyAppointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var doctorId = await GetDoctorIdFromToken();
            var appointments = await _doctorRepository.GetAppointmentsForDoctorAsync(doctorId);
            return Ok(appointments);
        }

        //  4. تقييم التزام المريض بالعلاج
        [HttpPost("EvaluateCompliance")]
        public async Task<IActionResult> EvaluatePatientCompliance([FromBody] PatientComplianceDTO dto)
        {
            var result = await _doctorRepository.EvaluatePatientComplianceAsync(dto);
            return result ? Ok(new { Message = "Patient compliance evaluated." }) : BadRequest();
        }

        //  5. عرض سجل عمل الطبيب
        [HttpGet("WorkHistory")]
        public async Task<IActionResult> GetWorkHistory()
        {
            var doctorId = await GetDoctorIdFromToken();
            var history = await _doctorRepository.GetWorkHistoryAsync(doctorId);
            return Ok(history);
        }

        //  6. عرض المهام اليومية
        [HttpGet("DailyTasks")]
        public async Task<IActionResult> GetDailyTasks()
        {
            var doctorId = await GetDoctorIdFromToken();
            var tasks = await _doctorRepository.GetDailyTasksAsync(doctorId);
            return Ok(tasks);
        }


        //  7. تقرير الأداء الشخصي
        [HttpGet("PerformanceReport")]
     
        public async Task<IActionResult> GetEnhancedPerformanceReport()
        {
            var doctorId = await GetDoctorIdFromToken();

            var score = await _doctorRepository.GetEnhancedPerformanceReportAsync(doctorId); 
 
            var rating = await _context.Feedbacks
                .Where(f => f.DoctorID == doctorId)
                .AverageAsync(f => (double?)f.Rating) ?? 0;

            return Ok(new
            {
                score = score,  
                rating = Math.Round(rating, 1) // 
            });
        }

        //  8. رفع ملف تقرير طبي للمريض
        [HttpPost("UploadPatientReport")]
        public async Task<IActionResult> UploadPatientReport([FromForm] UploadReportDTO dto)
        {
            var result = await _doctorRepository.UploadPatientReportAsync(dto);
            return result ? Ok(new { Message = "Report uploaded successfully." }) : BadRequest();
        }

        //  9. عرض السجل الكامل لمريض محدد
        [HttpGet("PatientFullHistory/{patientId}")]
        public async Task<IActionResult> GetPatientFullHistory(int patientId)
        {
            var history = await _doctorRepository.GetFullPatientHistoryAsync(patientId);
            return history != null ? Ok(history) : NotFound(new { Message = "Patient not found." });
        }

        //  10. إصدار تقرير PDF كامل لسجل المريض
        [HttpGet("GeneratePatientReport/{patientId}")]
        public async Task<IActionResult> GeneratePatientReport(int patientId)
        {
            var patientHistory = await _doctorRepository.GetFullPatientHistoryAsync(patientId);

            if (patientHistory == null)
                return NotFound(new { Message = "Patient not found." });

            var pdfBytes = _doctorRepository.GeneratePatientHistoryPdf(patientHistory);

            return File(pdfBytes, "application/pdf", $"Patient_{patientId}_FullReport.pdf");
        }

        [HttpGet("GetMyPatients")]
        public async Task<IActionResult> GetMyPatients()
        {
            var doctorId = await GetDoctorIdFromToken();
            var list = await _doctorRepository.GetMyPatientsAsync(doctorId);
            return Ok(list);
        }
     
        [HttpGet("DoctorHasNewMessages")]
        public async Task<IActionResult> DoctorHasNewMessages()
        {
            var doctorId = await GetDoctorIdFromToken();
            var hasNew = await _chatRepository.DoctorHasUnreadMessagesAsync(doctorId);
            return Ok(hasNew);
        }

        [HttpGet("GetMyChatPatients")]
        public async Task<IActionResult> GetMyChatPatients()
        {
            var doctorId = await GetDoctorIdFromToken();
            var userId = await _doctorRepository.GetUserIdByDoctorIdAsync(doctorId); 

            var list = await _doctorRepository.GetMyChatPatientsAsync(doctorId, userId);
            return Ok(list);
        }

        [HttpGet("PatientReports/{patientId}")]
        public async Task<IActionResult> GetPatientReports(int patientId)
        {
            var reports = await _doctorRepository.GetPatientReportsAsync(patientId);
            return Ok(reports);
        }

        [HttpPost("UploadReportFile")]
        public async Task<IActionResult> UploadReportFile([FromForm] UploadReportFileDTO dto)
        {
            var success = await _doctorRepository.UploadReportFileAsync(dto);
            return success ? Ok("Uploaded") : BadRequest("Failed");
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("MarkAppointmentCompleted/{id}")]
        public async Task<IActionResult> MarkAppointmentCompleted(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound("Appointment not found.");

            appointment.StatusID = 3; // ✅ Completed
            await _context.SaveChangesAsync();

            return Ok("Appointment marked as completed.");
        }


        [HttpPost("RequestReschedule")]
        public async Task<IActionResult> RequestReschedule([FromBody] RescheduleRequestDTO dto)
        {
            var appointment = await _context.Appointments.FindAsync(dto.AppointmentId);
            if (appointment == null) return NotFound("Appointment not found");

            var doctorId = await GetDoctorIdFromToken();
            if (appointment.DoctorID != doctorId)
                return Forbid("Not authorized to modify this appointment");

            var isEmergency = appointment.DateTime.Date == DateTime.Today;

            if (isEmergency)
            {
                appointment.DateTime = dto.NewDate;
                appointment.StatusID = 5; // Rescheduled مباشرة

                await _context.ActivityLogs.AddAsync(new ActivityLog
                {
                    UserId = doctorId,
                    Action = "Emergency Reschedule",
                    TableAffected = "Appointments",
                    Description = $"Appointment {dto.AppointmentId} rescheduled directly (emergency)",
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return Ok(new { message = "Appointment rescheduled directly (emergency)." });
            }
            else
            {
                var request = new RescheduleRequest
                {
                    AppointmentId = dto.AppointmentId,
                    RequestedDateTime = dto.NewDate,
                    Reason = dto.Reason,
                    DoctorId = doctorId,
                    Status = "Pending",
                    RequestedAt = DateTime.Now
                };

                appointment.StatusID = 1;  

                await _context.RescheduleRequests.AddAsync(request);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reschedule request sent to supervisor." });
            }


        }

        private async Task<int> GetDoctorIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub ||
                c.Type == "userid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User ID not found in token.");

            if (!int.TryParse(userIdClaim, out int userId))
                throw new Exception($"Invalid user ID format: {userIdClaim}");

            var doctor = await _context.Doctors
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                throw new Exception("Doctor not found for this user.");

            return doctor.DoctorID;
        }
  
    }
}
