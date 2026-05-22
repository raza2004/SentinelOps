using FluentValidation;
using MediatR;
using SentinelOps.Application.Common.Interfaces;
using SentinelOps.Application.Common.Models;
using SentinelOps.Domain.Entities;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Auth.Commands;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role
) : IRequest<AuthResponse>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => r is "Admin" or "Engineer" or "Viewer")
            .WithMessage("Role must be one of: Admin, Engineer, Viewer.");
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly IRepository<User> _users;

    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IAuthService authService,
        IRepository<User> users)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _users = users;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PasswordHash = _authService.HashPassword(command.Password),
            Role = command.Role,
            IsActive = true
        };

        await _users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = await _authService.GenerateTokenAsync(user);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }
}
