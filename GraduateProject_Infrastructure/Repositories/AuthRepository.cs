using Asp9_Project_Core.Interfaces;
using GraduateProject_Core.Interfaces;
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
        private readonly IEmailService _emailService;
        public AuthRepository(UserManager<Users> userManager, IEmailService emailService, SignInManager<Users> signInManager, IConfiguration configuration , AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.appDbContext = appDbContext;
            _emailService = emailService;
        }




        public async Task<string> SendOTPAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return "User not found";

            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntity = new OTPVerification
            {
                UserId = user.Id,
                Code = otp,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(200)

            };

            appDbContext.OTPVerifications.Add(otpEntity);
            await appDbContext.SaveChangesAsync();

            await _emailService.SendEmailAsync(email, "Your OTP Code", $"Your OTP code is: {otp}");
            return "OTP sent to your email.";
        }

        //public async Task<string> RegisterAsync(Users user, string password)
        //{
        //     user.IsVerified = false;

        //    var result = await userManager.CreateAsync(user, password);
        //    if (!result.Succeeded)
        //        return string.Join(",", result.Errors.Select(e => e.Description));

        //    var createdUser = await userManager.FindByNameAsync(user.UserName);
        //    if (createdUser == null)
        //        return "Error: User creation failed.";

        //    bool isPatient = true;

        //     if (isPatient)
        //    {
        //        var patientEntity = new Patient
        //        {
        //            UserId = createdUser.Id,
        //            CurrentStatus = "Active",
        //            ComplianceLevel = "Normal",
        //            DateOfBirth = createdUser.DateOfBirth ?? DateTime.UtcNow.AddYears(-30),
        //            Gender = createdUser.Gender,
        //            Phone = createdUser.PhoneNumber,
        //            Status = new AppointmentStatus { StatusName = "Pending" }
        //        };

        //        appDbContext.Patients.Add(patientEntity);

        //    }
        //    else
        //    {

        //        createdUser.IsVerified = true;
        //    }

        //    await appDbContext.SaveChangesAsync();
        //    return "User registered successfully.";
        //}
        public async Task<string> RegisterAsync(Users user, string password)
        {
            user.IsVerified = false;

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return string.Join(",", result.Errors.Select(e => e.Description));

            var createdUser = await userManager.FindByNameAsync(user.UserName);
            if (createdUser == null)
                return "Error: User creation failed.";

            bool isPatient = true;

            if (isPatient)
            {
                var patientEntity = new Patient
                {
                    UserId = createdUser.Id,
                    CurrentStatus = "Active",
                    ComplianceLevel = "Normal",
                    DateOfBirth = createdUser.DateOfBirth ?? DateTime.UtcNow.AddYears(-30),
                    Gender = createdUser.Gender,
                    Phone = createdUser.PhoneNumber,
                    Status = new AppointmentStatus { StatusName = "Pending" }
                };

                appDbContext.Patients.Add(patientEntity);

                // 🟢 أضف هذا السطر لإعطاء الدور للمستخدم
                await userManager.AddToRoleAsync(createdUser, "Patient");
            }
            else
            {
                createdUser.IsVerified = true;
            }

            await appDbContext.SaveChangesAsync();
            return "User registered successfully.";
        }


        public async Task<string> LoginAsync(string username, string password)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(username);
                if (user == null)
                    return null;

                var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
                if (!result.Succeeded)
                    return null;

                var isPatient = await appDbContext.Patients.AnyAsync(p => p.UserId == user.Id);
                if (isPatient && !user.IsVerified)
                    return "NOT_VERIFIED";

                return GenerateToken(user);
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        private string GenerateToken(Users user)
        {
            var claims = new List<Claim>
    {
         new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim("userid", user.Id.ToString()), 
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email)
       

        };

          
            var roles = userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("role", role));

            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT Key is missing")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

        public async Task<string> ResetPasswordWithOTPAsync(string username, string otp, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(username);
            if (user == null)
                return "User not found";

            var otpEntity = await appDbContext.OTPVerifications
                .Where(o => o.UserId == user.Id && o.Code == otp && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
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

        public async Task<bool> VerifyEmailAsync(string email, string code)
        {
            var otp = await appDbContext.OTPVerifications
                .Include(o => o.User)
                .FirstOrDefaultAsync(o =>
                    o.Code == code &&
                    o.User.Email == email &&
                    !o.IsUsed &&
                    o.ExpiresAt > DateTime.Now);

            if (otp == null)
                return false;

            otp.IsUsed = true;
            otp.User.IsVerified = true;
            await appDbContext.SaveChangesAsync();

            return true;
        }

    }
}