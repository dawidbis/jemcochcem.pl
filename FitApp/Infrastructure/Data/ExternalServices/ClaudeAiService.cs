namespace FitApp.Infrastructure.ExternalServices;

using FitApp.Application.Interfaces;
using FitApp.Application.DTOs;
using System.Net.Http;
using System.Threading.Tasks;

public class ClaudeAiService : IClaudeAiService
{
    private readonly HttpClient _httpClient;

    public ClaudeAiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<AiAnalysisResult> AnalyzeMealPhotoAsync(byte[] photoBytes) => throw new NotImplementedException();
    public Task<string> AskCoachAsync(string prompt) => throw new NotImplementedException();
}