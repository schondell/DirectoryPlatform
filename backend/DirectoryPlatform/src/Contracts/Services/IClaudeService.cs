namespace DirectoryPlatform.Contracts.Services;

public interface IClaudeService
{
    Task<string> SendMessageAsync(string systemPrompt, string userMessage);
}
