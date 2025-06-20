using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface IReportFileRepository
    {
        Task<bool> UploadReportAsync(UploadReportDTO dto);
        Task<IEnumerable<ReportFile>> GetReportsByPatientIdAsync(int patientId);
        Task<ReportFile?> GetReportByIdAsync(int reportId);
        Task<bool> DeleteReportAsync(int reportId);
    }
}
