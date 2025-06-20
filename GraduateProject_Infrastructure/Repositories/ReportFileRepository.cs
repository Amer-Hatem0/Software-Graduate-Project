using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class ReportFileRepository : IReportFileRepository
    {
        private readonly AppDbContext _context;

        public ReportFileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UploadReportAsync(UploadReportDTO dto)
        {
            if (dto.ReportFile == null || dto.ReportFile.Length == 0)
                return false;

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.ReportFile.FileName)}"; // اسم فريد لتجنب التكرار
            var uploadsFolder = Path.Combine("wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ReportFile.CopyToAsync(stream);
            }

            var report = new ReportFile
            {
                PatientID = dto.PatientId,
                FileName = dto.ReportFile.FileName,
                FileUrl = $"/uploads/{fileName}",
                UploadedAt = DateTime.UtcNow,
                Description = dto.Description,
                Specialization = dto.Specialization  
            };

            _context.ReportFiles.Add(report);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ReportFile>> GetReportsByPatientIdAsync(int patientId)
        {
            return await _context.ReportFiles
                .Where(r => r.PatientID == patientId)
                .ToListAsync();
        }

        public async Task<ReportFile?> GetReportByIdAsync(int reportId)
        {
            return await _context.ReportFiles.FindAsync(reportId);
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            var report = await _context.ReportFiles.FindAsync(reportId);
            if (report == null) return false;

            _context.ReportFiles.Remove(report);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
