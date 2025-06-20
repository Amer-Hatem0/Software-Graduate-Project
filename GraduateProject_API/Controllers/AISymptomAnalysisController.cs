using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AISymptomAnalysisController : ControllerBase
    {
        private readonly IAISymptomAnalysisRepository _repo;

        public AISymptomAnalysisController(IAISymptomAnalysisRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("Analyze")]
        public async Task<IActionResult> Analyze([FromBody] SymptomAnalysisRequestDTO dto)
        {
            var result = await _repo.AnalyzeSymptomsAsync(dto);
            return Ok(result);
        }
    }

}
