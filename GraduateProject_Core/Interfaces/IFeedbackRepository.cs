using GraduateProject_Core.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<bool> AddFeedbackAsync(FeedbackDTO dto);
        Task<IEnumerable<FeedbackDTO>> GetFeedbacksByPatientAsync(int patientId);
        Task<IEnumerable<FeedbackDTO>> GetFeedbacksForDoctorAsync(int doctorId);
        Task<IEnumerable<FeedbackDisplayDTO>> GetAllFeedbacksAsync();

    }
}
