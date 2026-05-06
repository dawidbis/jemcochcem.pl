namespace FitApp.Application.Interfaces;

using FitApp.Application.DTOs;
using System.Threading.Tasks;

public interface IClaudeAiService
{
    Task<AiAnalysisResult> AnalyzeMealPhotoAsync(byte[] photoBytes);
    Task<string> AskCoachAsync(string prompt);
}