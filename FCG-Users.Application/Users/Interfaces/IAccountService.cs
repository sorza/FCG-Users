using FCG_Users.Application.Shared.Results;
using FCG_Users.Application.Users.Requests;
using FCG_Users.Application.Users.Responses;

namespace FCG_Users.Application.Users.Interfaces
{
    public interface IAccountService
    {
        Task<Result<AccountResponse>> CreateAccountAsync(AccountRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> AuthAsync(AuthRequest request, CancellationToken cancellationToken = default);
    }
}
