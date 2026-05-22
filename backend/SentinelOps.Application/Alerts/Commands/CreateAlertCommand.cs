using MediatR;
using SentinelOps.Domain.Entities;
using SentinelOps.Domain.Enums;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Alerts.Commands;

public record CreateAlertCommand(
    string Title,
    string Message,
    string Source,
    Severity Severity,
    double? MetricValue,
    double? Threshold
) : IRequest<Guid>;

public class CreateAlertCommandHandler : IRequestHandler<CreateAlertCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAlertCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateAlertCommand command, CancellationToken cancellationToken)
    {
        var alert = new Alert
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Message = command.Message,
            Source = command.Source,
            Severity = command.Severity,
            Status = AlertStatus.Active,
            MetricValue = command.MetricValue,
            Threshold = command.Threshold
        };

        await _unitOfWork.Alerts.AddAsync(alert);
        await _unitOfWork.SaveChangesAsync();

        return alert.Id;
    }
}
