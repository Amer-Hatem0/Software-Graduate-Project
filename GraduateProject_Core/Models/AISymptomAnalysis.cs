using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class AISymptomAnalysis
    {
        [Key]
        public int AnalysisID { get; set; }
        public int PatientID { get; set; }
        public string Symptoms { get; set; }
        public string SuggestedDiagnosis { get; set; }
        public DateTime Date { get; set; }

        public Patient Patient { get; set; }

    }
}
