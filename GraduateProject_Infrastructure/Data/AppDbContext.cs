using GraduateProject_Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<Users, IdentityRole<int>, int>


    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //// Constructor for EF CLI migrations (design-time)
        public AppDbContext() : base(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=hospital-server.database.windows.net;Database=HospitalDB;User Id=adminuser;Password=Amer2002*;")
                .Options)
        {
        }

        public DbSet<Users> Users { get; set; }
  
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<AISymptomAnalysis> AISymptomAnalyses { get; set; }

        public DbSet<AIModelRunLog> AIModelRunLogs { get; set; }
        public DbSet<AISymptomTemplate> AISymptomTemplates { get; set; }

        public DbSet<OTPVerification> OTPVerifications { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveStatus> LeaveStatuses { get; set; }
        public DbSet<ReportFile> ReportFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorSpecialization>()
                .HasKey(ds => new { ds.DoctorID, ds.SpecializationID });

            modelBuilder.Entity<DoctorSpecialization>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSpecializations)
                .HasForeignKey(ds => ds.DoctorID);

            modelBuilder.Entity<DoctorSpecialization>()
                .HasOne(ds => ds.Specialization)
                .WithMany(s => s.DoctorSpecializations)
                .HasForeignKey(ds => ds.SpecializationID);

            base.OnModelCreating(modelBuilder);
        }

    }

}
