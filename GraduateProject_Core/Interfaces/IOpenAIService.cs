using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface IOpenAIService
    {
        Task<string> GetDiagnosisFromAI(string symptoms);
        Task<string> GetChatbotResponseAsync(string question);
    }
}
