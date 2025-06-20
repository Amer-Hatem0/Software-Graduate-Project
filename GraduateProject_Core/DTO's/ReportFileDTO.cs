using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class ReportFileDTO
    {
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string? Description { get; set; }
        public string? Specialization { get; set; }
        public DateTime UploadedAt { get; set; }
    }

}
