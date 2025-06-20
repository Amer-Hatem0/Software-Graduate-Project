using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class PatientProfileDTO
    {
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ComplianceLevel { get; set; }
        public string CurrentStatus { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? ImageUrl { get; set; }

    }
}
