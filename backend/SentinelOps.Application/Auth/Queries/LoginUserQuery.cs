using MediatR;
using SentinelOps.Application.Common.Interfaces;
using SentinelOps.Application.Common.Models;
using SentinelOps.Domain.Entities;
using SentinelOps.Domain.Interfaces;

namespace SentinelOps.Application.Auth.Queries;

public record LoginUserQuery(string Email, string Password) : IRequest<AuthResponse>;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly IRepository<User> _users;

    public LoginUserQueryHandler(IAuthService authService, IRepository<User> users)
    {
        _authService = authService;
        _users = users;
    }

    public async Task<AuthResponse> Handle(LoginUserQuery query, CancellationToken cancellationToken)
    {
        var all = await _users.GetAllAsync();
        var user = all.FirstOrDefault(u =>
            u.Email.Equals(query.Email, StringComparison.OrdinalIgnoreCase));

        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!_authService.VerifyPassword(query.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

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
