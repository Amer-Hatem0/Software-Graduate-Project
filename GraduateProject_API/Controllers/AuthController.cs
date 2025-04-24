using Asp9_Project_Core.Interfaces;
using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        this.authRepository = authRepository;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var user = new Users { UserName = dto.UserName, Email = dto.Email  };
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
}