using FCG_Users.Application.Shared.Results;
using FCG_Users.Application.Users.Requests;
using FCG_Users.Application.Users.Responses;

namespace FCG_Users.Application.Shared.Interfaces
{
    public interface IAccountService
    {
        Task<Result<AccountResponse>> CreateAccountAsync(AccountRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> AuthAsync(AuthRequest request, CancellationToken cancellationToken = default);
        Task<Result<AccountResponse>> GetUserAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> RemoveUserAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
