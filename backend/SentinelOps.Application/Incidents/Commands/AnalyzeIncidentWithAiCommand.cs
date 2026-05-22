using MediatR;
using SentinelOps.Application.Common.Interfaces;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Incidents.Commands;

public record AnalyzeIncidentWithAiCommand(Guid IncidentId) : IRequest<bool>;

public class AnalyzeIncidentWithAiCommandHandler
    : IRequestHandler<AnalyzeIncidentWithAiCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOpenAiService _openAiService;

    public AnalyzeIncidentWithAiCommandHandler(IUnitOfWork unitOfWork, IOpenAiService openAiService)
    {
        _unitOfWork = unitOfWork;
        _openAiService = openAiService;
    }

    public async Task<bool> Handle(
        AnalyzeIncidentWithAiCommand command, CancellationToken cancellationToken)
    {
        var incident = await _unitOfWork.Incidents.GetByIdAsync(command.IncidentId)
            ?? throw new KeyNotFoundException($"Incident {command.IncidentId} not found.");

        var alerts = await _unitOfWork.Alerts.GetByIncidentAsync(command.IncidentId);
        var alertMessages = alerts.Select(a => a.Message).ToList();

        var result = await _openAiService.AnalyzeIncidentAsync(
            incident.Title,
            incident.Description,
            incident.Severity.ToString(),
            alertMessages);

        incident.AiRootCause = result.RootCause;
        incident.AiSuggestedFix = result.SuggestedFix;
        incident.AiSeverityExplanation = result.SeverityExplanation;

        await _unitOfWork.Incidents.UpdateAsync(incident);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
