using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class AIModelRunLog
    {
        [Key]
        public int ID { get; set; }
        public string InputText { get; set; }
        public string OutputResult { get; set; }
        public string ModelUsed { get; set; }
        public DateTime RunTime { get; set; }

    }
}
