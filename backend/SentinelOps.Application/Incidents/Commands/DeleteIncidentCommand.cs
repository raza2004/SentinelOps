using MediatR;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Incidents.Commands;

public record DeleteIncidentCommand(Guid Id) : IRequest<bool>;

public class DeleteIncidentCommandHandler : IRequestHandler<DeleteIncidentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIncidentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteIncidentCommand command, CancellationToken cancellationToken)
    {
        var incident = await _unitOfWork.Incidents.GetByIdAsync(command.Id)
            ?? throw new KeyNotFoundException($"Incident {command.Id} not found.");

        await _unitOfWork.Incidents.DeleteAsync(incident.Id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
