using GraduateProject_Core.DTO_s;
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

    public AdminController(UserManager<Users> userManager, RoleManager<IdentityRole<int>> roleManager, AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }
    //[HttpPost("CreateUser")]
    //public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDTO dto)
    //{
    //    if (dto.Role != "Doctor" && dto.Role != "Supervisor")
    //        return BadRequest("Only Doctor or Supervisor roles are allowed.");

    //    var user = new Users
    //    {
    //        UserName = dto.UserName,
    //        Email = dto.Email,
    //        FullName = dto.FullName,
    //        PhoneNumber = dto.PhoneNumber,
    //        Gender = dto.Gender,
    //        DateOfBirth = dto.DateOfBirth,
    //        Age = dto.Age
    //    };

    //    var result = await _userManager.CreateAsync(user, dto.Password);
    //    if (!result.Succeeded)
    //        return BadRequest(result.Errors);

    //    await _userManager.AddToRoleAsync(user, dto.Role);

    //    if (dto.Role == "Doctor")
    //    {
    //        var doc = new Doctor { UserId = user.Id };
    //        _context.Doctors.Add(doc);
    //    }
    //    else if (dto.Role == "Supervisor")
    //    {
    //        var sup = new Supervisor { UserId = user.Id };
    //        _context.Supervisors.Add(sup);
    //    }

    //    await _context.SaveChangesAsync();
    //    return Ok(new
    //    {
    //        message = $"{dto.Role} account created successfully.",
    //        userId = user.Id
    //    });
    //}


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

}