using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext _context;

        public FeedbackRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<bool> AddFeedbackAsync(FeedbackDTO dto)
        //{
        //    var feedback = new Feedback
        //    {
        //        PatientID = dto.PatientID,
        //        DoctorID = dto.DoctorID,
        //        Rating = dto.Rating,
        //        Comment = dto.Comments,
        //        Date = System.DateTime.UtcNow
        //    };
        //    await _context.Feedbacks.AddAsync(feedback);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}


     public async Task<bool> AddFeedbackAsync(FeedbackDTO dto)
{
    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorID == dto.DoctorUserID);
    if (doctor == null) return false;

    var feedback = new Feedback
    {
        PatientID = dto.PatientID,
        DoctorID = doctor.DoctorID,
        Rating = dto.Rating,
        Comment = dto.Comment,
        Date = dto.Date
    };

    await _context.Feedbacks.AddAsync(feedback);
    await _context.SaveChangesAsync();
    return true;
}


        public async Task<IEnumerable<FeedbackDTO>> GetFeedbacksByPatientAsync(int patientId)
        {
            return await _context.Feedbacks
                .Where(f => f.PatientID == patientId)
                .Select(f => new FeedbackDTO
                {
                    FeedbackID = f.FeedbackID,
                    DoctorID = f.DoctorID,
                    PatientID = f.PatientID,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    Date = f.Date
                }).ToListAsync();
        }

        public async Task<IEnumerable<FeedbackDTO>> GetFeedbacksForDoctorAsync(int doctorId)
        {
            return await _context.Feedbacks
                .Where(f => f.DoctorID == doctorId)
                .Select(f => new FeedbackDTO
                {
                    FeedbackID = f.FeedbackID,
                    DoctorID = f.DoctorID,
                    PatientID = f.PatientID,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    Date = f.Date
                }).ToListAsync();
        }
        public async Task<IEnumerable<FeedbackDisplayDTO>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Patient)
                    .ThenInclude(p => p.User)
                .Include(f => f.Doctor)
                    .ThenInclude(d => d.User)
                .Select(f => new FeedbackDisplayDTO
                {
                    Id = f.FeedbackID,
                    PatientName = f.Patient.User.FullName,
                    DoctorName = f.Doctor.User.FullName,
                    Rating = f.Rating,
                    Message = f.Comment,
                    CreatedAt = f.Date
                })
                .ToListAsync();
        }

    }
}
