using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class PatientReport
    {
        [Key]
        public int PatientID { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public string Description { get; set; }
        public DateTime UploadedAt { get; set; }
    }


}
