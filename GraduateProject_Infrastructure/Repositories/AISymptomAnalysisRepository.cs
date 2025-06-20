using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class AISymptomAnalysisRepository : IAISymptomAnalysisRepository
    {
        private readonly AppDbContext _context;
        private readonly IOpenAIService _aiService;

        public AISymptomAnalysisRepository(AppDbContext context, IOpenAIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public async Task<SymptomAnalysisResponseDTO> AnalyzeSymptomsAsync(SymptomAnalysisRequestDTO dto)
        {
            var diagnosis = await _aiService.GetDiagnosisFromAI(dto.Symptoms);

            var record = new AISymptomAnalysis
            {
                PatientID = dto.PatientID,
                Symptoms = dto.Symptoms,
                SuggestedDiagnosis = diagnosis,
                Date = DateTime.UtcNow
            };

            _context.AISymptomAnalyses.Add(record);
            await _context.SaveChangesAsync();

            return new SymptomAnalysisResponseDTO
            {
                SuggestedDiagnosis = diagnosis,
                Date = record.Date
            };
        }
    }

}
