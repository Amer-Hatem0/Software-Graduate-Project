using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet("Profile/{patientId}")]
        public async Task<IActionResult> GetProfile(int patientId)
        {
            var profile = await _patientRepository.GetPatientProfileAsync(patientId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpGet("Doctors")]
        public async Task<IActionResult> GetAvailableDoctors()
        {
            var doctors = await _patientRepository.GetAvailableDoctorsAsync();
            return Ok(doctors);
        }

        [HttpPost("BookAppointment")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDTO dto)
        {
            var result = await _patientRepository.BookAppointmentAsync(dto);
            return result ? Ok(new { Message = "Appointment booked." }) : BadRequest();
        }

        [HttpGet("Appointments/{patientId}")]
        public async Task<IActionResult> GetAppointments(int patientId)
        {
            var appointments = await _patientRepository.GetAppointmentsAsync(patientId);
            return Ok(appointments);
        }

        [HttpDelete("CancelAppointment/{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var result = await _patientRepository.CancelAppointmentAsync(appointmentId);
            return result ? Ok(new { Message = "Appointment canceled." }) : NotFound();
        }

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

        [HttpGet("MedicalHistory/{patientId}")]
        public async Task<IActionResult> GetMedicalHistory(int patientId)
        {
            var history = await _patientRepository.GetMedicalHistoryAsync(patientId);
            return Ok(history);
        }
    }
}
