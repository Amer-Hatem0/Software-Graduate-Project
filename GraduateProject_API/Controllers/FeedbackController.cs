// FeedbackController.cs
using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly INotificationRepository _notificationRepository;


        public FeedbackController(IFeedbackRepository feedbackRepository, INotificationRepository notificationRepository)
        {
            _feedbackRepository = feedbackRepository;
            _notificationRepository = notificationRepository; 
        }


        [HttpPost("Add")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackDTO dto)
        {
            var result = await _feedbackRepository.AddFeedbackAsync(dto);
            if (result)
            {
                var notification = new Notification
                {
                    UserId = dto.DoctorUserID,
                    Title = "New Feedback 📝",
                    Message = "A patient has submitted feedback.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                await _notificationRepository.CreateNotificationAsync(notification);
                return Ok(new { Message = "Feedback added." });
            }

            return BadRequest();
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("Patient/{patientId}")]
        public async Task<IActionResult> GetFeedbacksByPatient(int patientId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByPatientAsync(patientId);
            return Ok(feedbacks);
        }
        [Authorize(Roles = "Patient,Doctor,Admin,Supervisor")]
        [HttpGet("Doctor/{doctorId}")]
        public async Task<IActionResult> GetFeedbacksForDoctor(int doctorId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksForDoctorAsync(doctorId);
            return Ok(feedbacks);
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _feedbackRepository.GetAllFeedbacksAsync();
            return Ok(feedbacks);
        }

    }
}
