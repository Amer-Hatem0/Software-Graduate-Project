using GraduateProject_Core.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface IAISymptomAnalysisRepository
    {
        Task<SymptomAnalysisResponseDTO> AnalyzeSymptomsAsync(SymptomAnalysisRequestDTO dto);
    }

}
