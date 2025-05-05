using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.DTO_s
{
    public class MedicalHistoryItemDTO
    {

        public string Disease { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
