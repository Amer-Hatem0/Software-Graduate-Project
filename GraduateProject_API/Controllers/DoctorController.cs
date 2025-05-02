using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Infrastructure.Data;
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
    [Authorize(Roles = "Doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(IDoctorRepository doctorRepository, AppDbContext context, ILogger<DoctorController> logger)
        {
            _doctorRepository = doctorRepository;
            _context = context;
            _logger = logger;
        }

        // ✅ 1. طلب إجازة
        [HttpPost("RequestLeave")]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveRequestDTO dto)
        {
            var doctorId = await GetDoctorIdFromToken();
            dto.DoctorID = doctorId;

            var result = await _doctorRepository.RequestLeaveAsync(dto);
            return result ? Ok(new { Message = "Leave request submitted." }) : BadRequest();
        }

        // ✅ 2. طلب إعادة جدولة موعد
        [HttpPost("RequestReschedule")]
        public async Task<IActionResult> RequestAppointmentReschedule([FromBody] RescheduleRequestDTO dto)
        {
            var result = await _doctorRepository.RequestAppointmentRescheduleAsync(dto);
            return result ? Ok(new { Message = "Reschedule request submitted." }) : BadRequest();
        }

        //// ✅ 3. إضافة ملاحظة للمريض
        //[HttpPost("AddPatientNote")]
        //public async Task<IActionResult> AddPatientNote([FromBody] PatientNoteDTO dto)
        //{
        //    var result = await _doctorRepository.AddPatientNoteAsync(dto);
        //    return result ? Ok(new { Message = "Note added to patient history." }) : BadRequest();
        //}

        [HttpPost("AddPatientNote")]
        public async Task<IActionResult> AddPatientNote([FromBody] PatientNoteDTO dto)
        {
            try
            {
                var doctorId = await GetDoctorIdFromToken();  // استخراج DoctorID
                var result = await _doctorRepository.AddPatientNoteAsync(dto, doctorId);

                return result ? Ok(new { Message = "Note added to patient history." }) : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding patient note.");
                return StatusCode(500, ex.Message);
            }
        }


        // ✅ 4. تقييم التزام المريض بالعلاج
        [HttpPost("EvaluateCompliance")]
        public async Task<IActionResult> EvaluatePatientCompliance([FromBody] PatientComplianceDTO dto)
        {
            var result = await _doctorRepository.EvaluatePatientComplianceAsync(dto);
            return result ? Ok(new { Message = "Patient compliance evaluated." }) : BadRequest();
        }

        // ✅ 5. عرض سجل عمل الطبيب
        [HttpGet("WorkHistory")]
        public async Task<IActionResult> GetWorkHistory()
        {
            var doctorId = await GetDoctorIdFromToken();
            var history = await _doctorRepository.GetWorkHistoryAsync(doctorId);
            return Ok(history);
        }

        // ✅ 6. عرض المهام اليومية
        [HttpGet("DailyTasks")]
        public async Task<IActionResult> GetDailyTasks()
        {
            var doctorId = await GetDoctorIdFromToken();
            var tasks = await _doctorRepository.GetDailyTasksAsync(doctorId);
            return Ok(tasks);
        }


        // ✅ 7. تقرير الأداء الشخصي
        [HttpGet("PerformanceReport")]
        public async Task<IActionResult> GetEnhancedPerformanceReport()
        {
            var doctorId = await GetDoctorIdFromToken();
            var report = await _doctorRepository.GetEnhancedPerformanceReportAsync(doctorId);
            return Ok(report);
        }

        // ✅ 8. رفع ملف تقرير طبي للمريض
        [HttpPost("UploadPatientReport")]
        public async Task<IActionResult> UploadPatientReport([FromForm] UploadReportDTO dto)
        {
            var result = await _doctorRepository.UploadPatientReportAsync(dto);
            return result ? Ok(new { Message = "Report uploaded successfully." }) : BadRequest();
        }

        // ✅ 9. عرض السجل الكامل لمريض محدد
        [HttpGet("PatientFullHistory/{patientId}")]
        public async Task<IActionResult> GetPatientFullHistory(int patientId)
        {
            var history = await _doctorRepository.GetFullPatientHistoryAsync(patientId);
            return history != null ? Ok(history) : NotFound(new { Message = "Patient not found." });
        }

        // ✅ 10. إصدار تقرير PDF كامل لسجل المريض
        [HttpGet("GeneratePatientReport/{patientId}")]
        public async Task<IActionResult> GeneratePatientReport(int patientId)
        {
            var patientHistory = await _doctorRepository.GetFullPatientHistoryAsync(patientId);

            if (patientHistory == null)
                return NotFound(new { Message = "Patient not found." });

            var pdfBytes = _doctorRepository.GeneratePatientHistoryPdf(patientHistory);

            return File(pdfBytes, "application/pdf", $"Patient_{patientId}_FullReport.pdf");
        }

        // 🔒 استخراج DoctorID من التوكن
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
