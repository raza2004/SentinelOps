using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using SentinelOps.Application.Common.Interfaces;

namespace SentinelOps.Infrastructure.Services;

public class OpenAiService : IOpenAiService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenAiService(IConfiguration configuration, ILogger<OpenAiService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AiAnalysisResult> AnalyzeIncidentAsync(
        string title,
        string description,
        string severity,
        List<string> alertMessages)
    {
        try
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var client = new ChatClient("gpt-4o", apiKey);

            var systemPrompt =
                "You are a senior DevOps engineer and incident response specialist. " +
                "Analyze incidents and provide structured JSON responses only.";

            var userPrompt =
                $"""
                Analyze this DevOps incident and respond with ONLY a JSON object with these exact fields:
                rootCause (string), suggestedFix (string), severityExplanation (string),
                runbookSteps (array of strings).

                Incident Title: {title}
                Severity: {severity}
                Description: {description}
                Related Alerts: {string.Join(", ", alertMessages)}

                Respond with valid JSON only, no markdown, no explanation.
                """;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            };

            var response = await client.CompleteChatAsync(messages);
            var content = response.Value.Content[0].Text;

            return JsonSerializer.Deserialize<AiAnalysisResult>(content, JsonOptions)
                ?? new AiAnalysisResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze incident with OpenAI. Title: {Title}", title);
            return new AiAnalysisResult();
        }
    }
}
