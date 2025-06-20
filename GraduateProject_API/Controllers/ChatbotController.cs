using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IOpenAIService _service;

        public ChatbotController(IOpenAIService service)
        {
            _service = service;
        }

        [HttpPost("Ask")]
        public async Task<IActionResult> AskAI([FromBody] ChatbotRequestDTO dto)
        {
            var answer = await _service.GetChatbotResponseAsync(dto.Question);
            return Ok(new ChatbotResponseDTO { Answer = answer });
        }
    }
}