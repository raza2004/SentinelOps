using MediatR;
using SentinelOps.Domain.Enums;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Incidents.Commands;

public record UpdateIncidentCommand(
    Guid Id,
    string? Title,
    string? Description,
    IncidentStatus? Status,
    Severity? Severity,
    Guid? AssignedToId
) : IRequest<bool>;

public class UpdateIncidentCommandHandler : IRequestHandler<UpdateIncidentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncidentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateIncidentCommand command, CancellationToken cancellationToken)
    {
        var incident = await _unitOfWork.Incidents.GetByIdAsync(command.Id)
            ?? throw new KeyNotFoundException($"Incident {command.Id} not found.");

        if (command.Title is not null)
            incident.Title = command.Title;

        if (command.Description is not null)
            incident.Description = command.Description;

        if (command.Status is not null)
        {
            incident.Status = command.Status.Value;
            if (command.Status == IncidentStatus.Resolved)
                incident.ResolvedAt = DateTime.UtcNow;
        }

        if (command.Severity is not null)
            incident.Severity = command.Severity.Value;

        if (command.AssignedToId is not null)
            incident.AssignedToId = command.AssignedToId;

        await _unitOfWork.Incidents.UpdateAsync(incident);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
