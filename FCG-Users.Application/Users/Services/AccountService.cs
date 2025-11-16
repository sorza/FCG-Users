using FCG.Shared.Contracts;
using FCG_Users.Application.Shared.Interfaces;
using FCG_Users.Application.Shared.Repositories;
using FCG_Users.Application.Shared.Results;
using FCG_Users.Application.Users.Requests;
using FCG_Users.Application.Users.Responses;
using FCG_Users.Domain.Users.Entities;
using FCG_Users.Domain.Users.Enums;
using Fgc.Domain.Usuario.ObjetosDeValor;

namespace FCG_Users.Application.Users.Services
{
    public class AccountService(IAccountRepository repository, IJwtTokenService jwtService, IEventPublisher publisher) : IAccountService
    {
        public async Task<Result<AuthResponse>> AuthAsync(AuthRequest request, CancellationToken cancellationToken = default)
        {
            var email = Email.Create(request.Email);

            var conta = await repository.Auth(email, cancellationToken);

            if (conta is null)
                return Result.Failure<AuthResponse>(new Error("401", "Credenciais inválidas."));

            if(!conta.Password.Verify(request.Password))
                return Result.Failure<AuthResponse>(new Error("401", "Credenciais inválidas."));

            if(!conta.Active)
                return Result.Failure<AuthResponse>(new Error("403", "Usuário inativo."));

            var tokenInfo = jwtService.CreateToken(conta);

            var response = new AuthResponse(tokenInfo.Token, tokenInfo.ExpiresAt);

            return Result.Success(response);

        }

        public async Task<Result<AccountResponse>> CreateAccountAsync(AccountRequest request, CancellationToken cancellationToken = default)
        {
            var userExists = await repository.Exists(request.Email, cancellationToken);

            if (userExists)
                return Result.Failure<AccountResponse>(new Error("409", "Este usuário já está cadastrado."));

            var account = Account.Create(request.Name, request.Password, request.Email, EProfileType.Common);

            await repository.CreateAsync(account, cancellationToken);

            var evt = new UserCreatedEvent(account.Id, account.Name, account.Email, account.Profile.ToString(), account.Active);
            await publisher.PublishAsync(evt, "UserCreated");

            return Result.Success(new AccountResponse(
                account.Id,
                account.Name,
                account.Password,
                account.Email,
                account.Profile,
                account.Active
            ));

        }

        public async Task<Result<AccountResponse>> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await repository.GetByIdAsync(id);
            if (user is null)
                return Result.Failure<AccountResponse>(new Error("404", "Usuário não encontrado."));

            return Result.Success(new AccountResponse(
                user.Id,
                user.Name,
                user.Password,
                user.Email,
                user.Profile,
                user.Active
                ));            
        }

        public async Task<Result> RemoveUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var usuario = await repository.GetByIdAsync(id);

            if (usuario is null)
                return Result.Failure(new Error("404", "Usuário não encontrado"));

            await repository.DeleteAsync(id);
            
            var evt = new UserDeletedEvent(usuario.Id);
            await publisher.PublishAsync(evt, "UserDeleted");

            return Result.Success(new AccountResponse(
                usuario.Id,
                usuario.Name,
                usuario.Password,
                usuario.Email,
                usuario.Profile,
                usuario.Active
                ));
        }
    }
}
