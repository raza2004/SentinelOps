using FluentValidation;
using MediatR;
using SentinelOps.Domain.Entities;
using SentinelOps.Domain.Enums;
using SentinelOps.Domain.Interfaces;
using DomainSeverity = SentinelOps.Domain.Enums.Severity;

namespace SentinelOps.Application.Incidents.Commands;

public record CreateIncidentCommand(
    string Title,
    string Description,
    DomainSeverity Severity,
    Guid? AssignedToId,
    int SlaHours = 4
) : IRequest<Guid>;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.SlaHours)
            .InclusiveBetween(1, 168);
    }
}

public class CreateIncidentCommandHandler : IRequestHandler<CreateIncidentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateIncidentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateIncidentCommand command, CancellationToken cancellationToken)
    {
        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Severity = command.Severity,
            AssignedToId = command.AssignedToId,
            Status = IncidentStatus.Open,
            SlaDeadline = DateTime.UtcNow.AddHours(command.SlaHours)
        };

        await _unitOfWork.Incidents.AddAsync(incident);
        await _unitOfWork.SaveChangesAsync();

        return incident.Id;
    }
}
