using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class ReportFDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public string PatientName { get; set; }
    }

}
