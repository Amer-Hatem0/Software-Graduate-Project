using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class ReportFile
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        public int PatientID { get; set; }

        [Required]
        [StringLength(150)]
        public string FileName { get; set; }

        [Required]
        public string FileUrl { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Patient Patient { get; set; }


    }
}