namespace Backend.services;
using Backend.dtos;
public interface IClaudeAiService
{
    Task<string> AskCoach(string msg);
    Task<AiResultDto> AnalyzePhoto(byte[] msg);
}