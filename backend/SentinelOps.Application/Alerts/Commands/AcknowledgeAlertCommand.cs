using MediatR;
using SentinelOps.Domain.Enums;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Alerts.Commands;

public record AcknowledgeAlertCommand(Guid Id) : IRequest<bool>;

public class AcknowledgeAlertCommandHandler : IRequestHandler<AcknowledgeAlertCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AcknowledgeAlertCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AcknowledgeAlertCommand command, CancellationToken cancellationToken)
    {
        var alert = await _unitOfWork.Alerts.GetByIdAsync(command.Id)
            ?? throw new KeyNotFoundException($"Alert {command.Id} not found.");

        alert.Status = AlertStatus.Acknowledged;

        await _unitOfWork.Alerts.UpdateAsync(alert);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
