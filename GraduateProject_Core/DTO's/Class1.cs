using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class UploadReportFileDTO
    {
        public int PatientId { get; set; }
        public IFormFile ReportFile { get; set; }
        public string? Specialization { get; set; }
        public string? Description { get; set; }
    }

}
