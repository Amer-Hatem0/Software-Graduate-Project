using Asp9_Project_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext appDbContext;

        public AuthRepository(UserManager<Users> userManager, SignInManager<Users> signInManager, IConfiguration configuration , AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.appDbContext = appDbContext;
        }
        //public async Task<string> RegisterAsync(Users user, string password)
        //{
        //    var result = await userManager.CreateAsync(user, password);
        //    if (result.Succeeded)
        //    {
        //        return "User Registed successfully ";
        //    }
        //    var err = result.Errors.Select(result => result.Description).ToList();
        //    return string.Join(",", err);

        //}

        //public async Task<string> RegisterAsync(Users user, string password)
        //{
        //    var result = await userManager.CreateAsync(user, password);
        //    if (!result.Succeeded)
        //        return string.Join(",", result.Errors.Select(e => e.Description));

        //    await userManager.AddToRoleAsync(user, "Patient");

        //    var patientEntity = new Patient
        //    {
        //        UserId = user.Id
        //    };
        //    appDbContext.Patients.Add(patientEntity);
        //    await appDbContext.SaveChangesAsync();

        //    return "Patient registered successfully";
        //}

        public async Task<string> RegisterAsync(Users user, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return string.Join(",", result.Errors.Select(e => e.Description));

            await userManager.AddToRoleAsync(user, "Patient");

            var patientEntity = new Patient
            {
                UserId = user.Id
            };
            appDbContext.Patients.Add(patientEntity);
            await appDbContext.SaveChangesAsync();

            return "Patient registered successfully";
        }

        public async Task<string> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return "User not found";

            var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            return "Password changed successfully";
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
          
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configuration["JWT:Issuer"],
                configuration["JWT:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: cred
            );
            

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<string> SendOTPAsync(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
                return "User not found";

            var otp = new Random().Next(100000, 999999).ToString();

            var otpEntity = new OTPVerification
            {
                UserId = user.Id,
                Code = otp,
                IsUsed = false,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(5)
            };

            appDbContext.OTPVerifications.Add(otpEntity);
            await appDbContext.SaveChangesAsync();

            // هنا ممكن تطبع الرمز أو تبعته على الإيميل الحقيقي
            return $"OTP code sent: {otp}";
        }

        public async Task<string> ResetPasswordWithOTPAsync(string username, string otp, string newPassword)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
                return "User not found";

            var otpEntity = await appDbContext.OTPVerifications
                .Where(o => o.UserId == user.Id && o.Code == otp && !o.IsUsed && o.ExpiresAt > DateTime.Now)
                .FirstOrDefaultAsync();

            if (otpEntity == null)
                return "Invalid or expired OTP";

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            otpEntity.IsUsed = true;
            await appDbContext.SaveChangesAsync();

            return "Password reset successfully";
        }


    }
}