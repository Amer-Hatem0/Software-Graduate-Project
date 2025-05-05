using Asp9_Project_Core.Interfaces;
using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthRepository authRepository;

    public AccountController(IAuthRepository authRepository)
    {
        this.authRepository = authRepository;
    }

 


    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var user = new Users
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            Age = dto.Age
        };

        var result = await authRepository.RegisterAsync(user, dto.Password);
        return Ok(new { message = result });
    }


  




    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var token = await authRepository.LoginAsync(dto.UserName, dto.Password);
        if (token == null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(new { token });
    }


    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var result = await authRepository.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);

        if (result.Contains("successfully"))
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOTP([FromBody] SendOTPModel model)
    {
        var result = await authRepository.SendOTPAsync(model.Username);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithOTPModel model)
    {
        var result = await authRepository.ResetPasswordWithOTPAsync(model.Username, model.OTP, model.NewPassword);
        return Ok(result);
    }


}