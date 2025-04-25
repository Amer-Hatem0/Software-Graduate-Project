 
using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asp9_Project_Core.Interfaces
{
    public interface IAuthRepository
    {

        Task<string> RegisterAsync(Users user, string password);
        Task<string> LoginAsync(string username, string password);
        Task<string> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<string> SendOTPAsync(string username);
        Task<string> ResetPasswordWithOTPAsync(string username, string otp, string newPassword);

    }
}