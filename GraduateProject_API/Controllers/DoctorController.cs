using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Doctor")]
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

        //[HttpPost("RequestLeave")]
        //public async Task<IActionResult> RequestLeave([FromBody] LeaveRequestDTO dto)
        //{
        //    int doctorId = await GetDoctorIdFromToken();
        //    dto.DoctorID = doctorId;

        //    var result = await _doctorRepository.RequestLeaveAsync(dto);

        //    if (!result)
        //        return BadRequest(new { Message = "Failed to request leave." });

        //    return Ok(new { Message = "Leave request submitted successfully." });
        //}





        [HttpPost("RequestLeave")]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveRequestDTO dto)
        {
            try
            {
                var userId = await GetDoctorIdFromToken();
                dto.DoctorID = userId; // تغيير من DoctorID إلى UserId إذا لزم الأمر

                var result = await _doctorRepository.RequestLeaveAsync(dto);

                return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RequestLeave");
                return StatusCode(500, ex.Message);
            }
        }

        //private async Task<int> GetDoctorIdFromToken()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (userIdClaim == null)
        //        throw new Exception("User ID not found in token.");

        //    int userId = int.Parse(userIdClaim);

        //    // الآن نحتاج أن نبحث عن Doctor الذي يملك هذا UserId
        //    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
        //    if (doctor == null)
        //        throw new Exception("Doctor not found for this user.");

        //    return doctor.DoctorID;
        //}


        private async Task<int> GetDoctorIdFromToken()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDoctorIdFromToken");
                throw;
            }
        }
    }
}
