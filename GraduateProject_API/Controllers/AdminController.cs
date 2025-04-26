using Asp9_Project_Core.Interfaces;
using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly UserManager<Users> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly AppDbContext _context;
    private readonly IAdminRepository adminRepository;

    public AdminController(UserManager<Users> userManager, RoleManager<IdentityRole<int>> roleManager, AppDbContext context, IAdminRepository adminRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        this.adminRepository = adminRepository;
    }





    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDTO dto)
    {
        if (!new[] { "Doctor", "Supervisor", "Admin" }.Contains(dto.Role))
            return BadRequest("Invalid role.");

        var user = new Users
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            Age = dto.Age,
            Specialization = dto.Specialization

        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, dto.Role);

        if (dto.Role == "Doctor")
        {
            _context.Doctors.Add(new Doctor { UserId = user.Id });
        }
        else if (dto.Role == "Supervisor")
        {
            _context.Supervisors.Add(new Supervisor { UserId = user.Id });
        }
        else if (dto.Role == "Admin")
        {
            _context.Admins.Add(new Admin { UserId = user.Id });
        }

        await _context.SaveChangesAsync();
        return Ok($"{dto.Role} account created successfully.");
    }


    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await adminRepository.GetAllUsersAsync();
        return Ok(users);
    }





    [HttpGet("GetUser/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await adminRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(200, new { Message = "User not found." });
        }
    }

            [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
         
            var result = await adminRepository.DeleteUserAsync(id);
            if (!result)
                return NotFound(new { Message = "User not found." });

            return Ok(new { Message = "User deleted successfully." });
         
        //catch (DbUpdateException ex)
        //{
        //    // تسجيل الخطأ للتحليل لاحقاً
        //    _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
        //    return StatusCode(500, new { Message = "Cannot delete user because it has related records. Please remove all roles first." });
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Unexpected error deleting user with ID {UserId}", id);
        //    return StatusCode(500, new { Message = "An unexpected error occurred." });
        //}
    }

    // ✅ عرض كل طلبات الإجازة
    [HttpGet("GetAllLeaveRequests")]
    public async Task<IEnumerable<LeaveRequestSimpleDTO>> GetAllLeaveRequestsAsync()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Doctor)
            .Include(lr => lr.Status)
            .Select(lr => new LeaveRequestSimpleDTO
            {
                RequestID = lr.RequestID,
                DoctorName = lr.Doctor.User.FullName,
                Reason = lr.Reason,
                Status = lr.Status.StatusName,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                SubmittedAt = lr.SubmittedAt
            })
            .ToListAsync();
    }


    // ✅ الموافقة على طلب إجازة
    [HttpPut("ApproveLeaveRequest/{leaveRequestId}")]
    public async Task<IActionResult> ApproveLeaveRequest(int leaveRequestId)
    {
        var result = await adminRepository.ApproveLeaveRequestAsync(leaveRequestId);
        if (!result)
            return NotFound(new { Message = "Leave request not found." });

        return Ok(new { Message = "Leave request approved." });
    }

    // ✅ رفض طلب إجازة
    [HttpPut("RejectLeaveRequest/{leaveRequestId}")]
    public async Task<IActionResult> RejectLeaveRequest(int leaveRequestId)
    {
        var result = await adminRepository.RejectLeaveRequestAsync(leaveRequestId);
        if (!result)
            return NotFound(new { Message = "Leave request not found." });

        return Ok(new { Message = "Leave request rejected." });
    }

    // ✅ عرض جميع طلبات الجرد الطبي
    [HttpGet("GetAllInventoryRequests")]
    public async Task<IActionResult> GetAllInventoryRequests()
    {
        var inventoryItems = await adminRepository.GetAllInventoryRequestsAsync();
        return Ok(inventoryItems);
    }

    // ✅ الموافقة على طلب جرد
    [HttpPut("ApproveInventoryRequest/{inventoryId}")]
    public async Task<IActionResult> ApproveInventoryRequest(int inventoryId)
    {
        var result = await adminRepository.ApproveInventoryRequestAsync(inventoryId);
        if (!result)
            return NotFound(new { Message = "Inventory item not found." });

        return Ok(new { Message = "Inventory request approved." });
    }

    // ✅ رفض طلب جرد
    [HttpPut("RejectInventoryRequest/{inventoryId}")]
    public async Task<IActionResult> RejectInventoryRequest(int inventoryId)
    {
        var result = await adminRepository.RejectInventoryRequestAsync(inventoryId);
        if (!result)
            return NotFound(new { Message = "Inventory item not found." });

        return Ok(new { Message = "Inventory request rejected." });
    }

    // ✅ تعديل طلب جرد
    [HttpPut("EditInventoryRequest/{inventoryId}")]
    public async Task<IActionResult> EditInventoryRequest(int inventoryId, [FromBody] InventoryUpdateDTO dto)
    {
        var result = await adminRepository.EditInventoryRequestAsync(inventoryId, dto);
        if (!result)
            return NotFound(new { Message = "Inventory item not found." });

        return Ok(new { Message = "Inventory updated successfully." });
    }

    // ✅ عرض الإحصاءات الكاملة
    [HttpGet("GetHospitalStatistics")]
    public async Task<IActionResult> GetHospitalStatistics()
    {
        var stats = await adminRepository.GetHospitalStatisticsAsync();
        return Ok(stats);
    }
}