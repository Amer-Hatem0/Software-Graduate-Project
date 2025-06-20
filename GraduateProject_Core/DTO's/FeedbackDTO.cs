using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class FeedbackDTO
    {
        public int FeedbackID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public int DoctorUserID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }  
        public DateTime Date { get; set; }
    }
}
