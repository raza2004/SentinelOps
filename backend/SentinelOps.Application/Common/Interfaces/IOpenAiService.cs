namespace SentinelOps.Application.Common.Interfaces;

public interface IOpenAiService
{
    Task<AiAnalysisResult> AnalyzeIncidentAsync(
        string title,
        string description,
        string severity,
        List<string> alertMessages);
}

public class AiAnalysisResult
{
    public string RootCause { get; set; } = string.Empty;
    public string SuggestedFix { get; set; } = string.Empty;
    public string SeverityExplanation { get; set; } = string.Empty;
    public List<string> RunbookSteps { get; set; } = new();
}
