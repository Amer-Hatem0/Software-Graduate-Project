using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace GraduateProject_Core.DTO_s
{
    

    public class PatientDoctorAssignmentDTO
    {
        [JsonPropertyName("patientId")]
        public int PatientId { get; set; }

        [JsonPropertyName("doctorId")]
        public int DoctorId { get; set; }
        public DateTime DateTime { get; set; }
    }

}
