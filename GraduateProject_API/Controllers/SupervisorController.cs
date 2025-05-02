using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : ControllerBase
    {

    
        private readonly AppDbContext _context;
       
        private readonly ISupervisorRepository _supervisorRepository;

        public SupervisorController(AppDbContext context, ISupervisorRepository supervisorRepository)
        {
            _supervisorRepository = supervisorRepository;
            _context = context;
        }

        [HttpPost("VerifyPatient/{patientId}")]
        public async Task<IActionResult> VerifyPatient(int patientId)
        {
            var result = await _supervisorRepository.VerifyNewPatientAsync(patientId);
            return result ? Ok(new { Message = "Patient verified." }) : NotFound(new { Message = "Patient not found." });
        }

        [HttpGet("Patients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _supervisorRepository.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpPost("AssignPatient")]
        public async Task<IActionResult> AssignPatient([FromBody] PatientDoctorAssignmentDTO dto)
        {
            var result = await _supervisorRepository.AssignPatientToDoctorAsync(dto);
            return result ? Ok(new { Message = "Patient assigned to doctor." }) : BadRequest();
        }

        [HttpGet("DoctorLeaveRequests")]
        public async Task<IActionResult> GetDoctorLeaveRequests()
        {
            var requests = await _supervisorRepository.GetAllDoctorLeaveRequestsAsync();
            return Ok(requests);
        }

        [HttpPut("ForwardLeaveRequest/{leaveRequestId}")]
        public async Task<IActionResult> ForwardLeaveRequest(int leaveRequestId)
        {
            var result = await _supervisorRepository.ForwardLeaveRequestToAdminAsync(leaveRequestId);
            return result ? Ok(new { Message = "Leave request forwarded to admin." }) : NotFound();
        }

        [HttpGet("Inventory")]
        public async Task<IActionResult> GetAllInventory()
        {
            var inventory = await _supervisorRepository.GetAllInventoryAsync();
            return Ok(inventory);
        }

        [HttpPost("SubmitInventory")]
        public async Task<IActionResult> SubmitInventoryRequest([FromBody] InventoryRequestDTO dto)  
        {
            var result = await _supervisorRepository.SubmitInventoryRequestAsync(dto);
            return result ? Ok(new { Message = "Inventory request submitted." }) : BadRequest();
        }


        [HttpGet("DailyOverview")]
        public async Task<IActionResult> GetDailyOverview()
        {
            var supervisorId = await GetSupervisorIdFromToken(); 
            var overview = await _supervisorRepository.GetDailyOverviewAsync(supervisorId);
            return Ok(overview);
        }


        private async Task<int> GetSupervisorIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub ||
                c.Type == "userid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User ID not found in token.");

            if (!int.TryParse(userIdClaim, out int userId))
                throw new Exception($"Invalid user ID format: {userIdClaim}");

            var doctor = await _context.Supervisors
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                throw new Exception("Supervisor not found for this user.");

            return doctor.SupervisorID;
        }

    }
}
