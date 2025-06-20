using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient,Doctor")]
    public class ReportFileController : ControllerBase
    {
        private readonly IReportFileRepository _repository;
        private readonly INotificationRepository _notificationRepository;

        public ReportFileController(IReportFileRepository repository, INotificationRepository notificationRepository)
        {
            _repository = repository;
            _notificationRepository = notificationRepository;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadReport([FromForm] UploadReportDTO dto)
        {
            var result = await _repository.UploadReportAsync(dto);
            if (result)
            {
                var notification = new Notification
                {
                    UserId = dto.DoctorUserId,
                    Title = "New Report Uploaded 📤",
                    Message = $"A new medical report has been uploaded for patient ID: {dto.PatientId}.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                await _notificationRepository.CreateNotificationAsync(notification);
                return Ok(new { Message = "Report uploaded successfully." });
            }

            return BadRequest("Upload failed.");
        }


        [HttpGet("Patient/{id}")]
        public async Task<IActionResult> GetReports(int id)
        {
            var reports = await _repository.GetReportsByPatientIdAsync(id);
            return Ok(reports);
        }

        [HttpGet("Download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var report = await _repository.GetReportByIdAsync(id);
            if (report == null) return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", report.FileUrl.TrimStart('/'));
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", report.FileName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repository.DeleteReportAsync(id);
            return result ? Ok(new { Message = "Deleted." }) : NotFound();
        }
    }
}
