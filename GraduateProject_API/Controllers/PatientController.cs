using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Infrastructure.Data;
using GraduateProject_Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly AppDbContext _context;

        public PatientController(IPatientRepository patientRepository , AppDbContext context)
        {
            _patientRepository = patientRepository;
            _context = context;
        }



        [HttpGet("LeaveRequests/{doctorId}")]
        public async Task<IActionResult> GetLeaveRequestsByDoctor(int doctorId)
        {
            var leaveRequests = await _patientRepository.GetLeaveRequestsByDoctorAsync(doctorId);
            return Ok(leaveRequests);
        }

        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Select(d => new
                {
                    d.DoctorID,
                    d.UserId,
                    FullName = d.User.FullName
                })
                .ToListAsync();

            return Ok(doctors);
        }
        [HttpGet("ProfileByUserId/{userId}")]
        public async Task<IActionResult> GetProfileByUserId(int userId)
        {
            var profile = await _patientRepository.GetPatientProfileAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }


        [HttpGet("Profile/{patientId}")]
        public async Task<IActionResult> GetProfile(int patientId)
        {
            var profile = await _patientRepository.GetPatientProfileAsync(patientId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }
        [HttpGet("PatientIdByUserId/{userId}")]
        
        public async Task<IActionResult> GetPatientIdByUserId(int userId)
        {
            var patientId = await _patientRepository.GetPatientIdByUserIdAsync(userId);
            if (patientId == null)
                return NotFound("Patient not found for the given User ID.");

            return Ok(new { patientId });
        }

        [HttpGet("Doctors")]
        public async Task<IActionResult> GetAvailableDoctors()
        {
            var doctors = await _patientRepository.GetAvailableDoctorsAsync();
            return Ok(doctors);
        }


        [HttpGet("DoctorIdByUserId/{userId}")]
        [Authorize(Roles = "Admin,Supervisor,Patient,Doctor")]
        public async Task<IActionResult> GetDoctorIdByUserId(int userId)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorID == userId);
            if (doctor == null) return NotFound();
            return Ok(new { doctorId = doctor.DoctorID });
        }


        //[HttpPost("BookAppointment")]
        //public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDTO dto)
        //{
        //    var result = await _patientRepository.BookAppointmentAsync(dto);
        //    return result ? Ok(new { Message = "Appointment booked." }) : BadRequest();
        //}

        //[HttpGet("Appointments/{patientId}")]
        //public async Task<IActionResult> GetAppointments(int patientId)
        //{
        //    var appointments = await _patientRepository.GetAppointmentsAsync(patientId);
        //    return Ok(appointments);
        //}

        //[HttpDelete("CancelAppointment/{appointmentId}")]
        //public async Task<IActionResult> CancelAppointment(int appointmentId)
        //{
        //    var result = await _patientRepository.CancelAppointmentAsync(appointmentId);
        //    return result ? Ok(new { Message = "Appointment canceled." }) : NotFound();
        //}

        [HttpPost("UploadReport")]
        public async Task<IActionResult> UploadReport([FromForm] UploadReportDTO dto)
        {
            var result = await _patientRepository.UploadReportAsync(dto);
            return result ? Ok(new { Message = "Report uploaded." }) : BadRequest();
        }

        [HttpGet("Reports/{patientId}")]
        public async Task<IActionResult> GetReports(int patientId)
        {
            var reports = await _patientRepository.GetReportsAsync(patientId);
            return Ok(reports);
        }

        //[HttpPut("Profile/{userId}")]
        //public async Task<IActionResult> UpdateProfile(int userId, [FromBody] PatientProfileDTO dto)
        //{
        //    var result = await _patientRepository.UpdatePatientProfileAsync(userId, dto);
        //    if (!result) return NotFound();
        //    return Ok(new { message = "Profile updated successfully." });
        //}

        [HttpPut("Profile/{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, [FromForm] PatientProfileDTO dto)
        {
            var result = await _patientRepository.UpdatePatientProfileAsync(userId, dto, dto.ProfileImage);
          if (!result) return NotFound();

            return Ok(new { message = "Profile updated successfully.", imageUrl = dto.ProfileImage?.FileName });
        }




        [HttpGet("MedicalHistory/{patientId}")]
        public async Task<IActionResult> GetMedicalHistory(int patientId)
        {
            var history = await _patientRepository.GetMedicalHistoryAsync(patientId);
            return Ok(history);
        }
    }
}
