using Asp9_Project_Core.Interfaces;
using GraduateProject_Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;
        private readonly IConfiguration configuration;

        public AuthRepository(UserManager<Users> userManager, SignInManager<Users> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }
        public async Task<string> RegisterAsync(Users user, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return "User Registed successfully ";
            }
            var err = result.Errors.Select(result => result.Description).ToList();
            return string.Join(",", err);

        }
        public Task<string> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            try
            {
                var user = await userManager.FindByNameAsync(username);
                if (user == null)
                    return null;

                var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
               

                if (!result.Succeeded)
                    return null;
              

                return GenerateToken(user);

            }
            catch (Exception ex)
            {
              

                return $"ERROR: {ex.Message}";
                

            }
        }

        private string GenerateToken(Users users)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, users.UserName),
        new Claim(ClaimTypes.NameIdentifier, users.Id.ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            Console.WriteLine("JWT Key: " + configuration["JWT:Key"]);

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configuration["JWT:Issuer"],
                configuration["JWT:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: cred
            );
            Console.WriteLine("JWT Key: " + configuration["JWT:Key"]);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}