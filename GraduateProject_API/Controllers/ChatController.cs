using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
namespace GraduateProject_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IChatRepository _chatRepository;

        public ChatController(AppDbContext context, IChatRepository chatRepository)
        {
            _context = context;
            _chatRepository = chatRepository;
        }

        [HttpGet("conversation/{user1Id}/{user2Id}")]
        public async Task<IActionResult> GetConversation(int user1Id, int user2Id)
        {
            var messages = await _chatRepository.GetConversationAsync(user1Id, user2Id);
            return Ok(messages);
        }

        [HttpGet("UnreadCountPerSender")]
  
        public async Task<IActionResult> GetUnreadCountPerSender()
        {
            var doctorId = await GetDoctorIdFromToken(); 
            var unread = await _chatRepository.GetUnreadCountsGroupedBySenderAsync(doctorId);
            return Ok(unread);  
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


        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessageDTO messageDto)
        {
            var result = await _chatRepository.SendMessageAsync(messageDto);
            return Ok(result);
        }
        [HttpPut("MarkAsRead/{messageId}")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            var success = await _chatRepository.MarkMessageAsReadAsync(messageId);
            return success ? Ok(new { Message = "Message marked as read." }) : NotFound();
        }
        [HttpPut("MarkAllFromSenderAsRead/{senderId}")]
        public async Task<IActionResult> MarkAllFromSenderAsRead(int senderId)
        {
            var currentUserId = int.Parse(User.Claims.First(c => c.Type == "sub" || c.Type == "userid").Value);
            var success = await _chatRepository.MarkAllMessagesFromSenderAsReadAsync(senderId, currentUserId);
            return success ? Ok(new { Message = "All messages marked as read." }) : NotFound();
        }

    }

}
