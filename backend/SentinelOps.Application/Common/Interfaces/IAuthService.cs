using SentinelOps.Domain.Entities;

namespace SentinelOps.Application.Common.Interfaces;

public interface IAuthService
{
    Task<string> GenerateTokenAsync(User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
