using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class PatientFullHistoryDTO
    {
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public int Phone { get; set; }
        public string Gender { get; set; }
        public List<VisitDTO> Visits { get; set; }
        public ICollection<MedicalHistoryItemDTO> MedicalHistories { get; set; }
          public AppointmentStatus Status { get; set; }
        public string StatusName { get; set; }
        public int AppointmentId { get ; set; }
        public Appointment Appointment { get; set; }
    }
}
