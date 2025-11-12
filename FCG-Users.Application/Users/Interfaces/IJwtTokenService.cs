using FCG_Users.Domain.Users.Entities;

namespace FCG_Users.Application.Users.Interfaces
{
    public interface IJwtTokenService
    {
        TokenInfo CreateToken(Account user);
    }
    public record TokenInfo(string Token, DateTime ExpiresAt);
}
