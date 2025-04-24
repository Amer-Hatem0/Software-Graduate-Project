using Microsoft.EntityFrameworkCore;
using GraduateProject_Infrastructure.Data;
using System.Globalization;
using Asp9_Project_Core.Interfaces;
using GraduateProject_Infrastructure.Repositories;
using GraduateProject_Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GraduateProject_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // تحميل إعدادات appsettings + appsettings.Development إذا موجود
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // الطباعة للتأكد
            Console.WriteLine("Loaded JWT Key: " + builder.Configuration["JWT:Key"]);

            // إضافة الخدمات
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<Users, IdentityRole<int>>(options =>
            {
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            })
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddScoped<IAuthRepository, AuthRepository>();

            // إضافة مصادقة JWT
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                    };
                });

            // ضبط اللغة الافتراضية
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // 🔐 يجب أن يكون قبل Authorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
