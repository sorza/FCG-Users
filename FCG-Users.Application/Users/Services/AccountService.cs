using FCG_Users.Application.Shared.Repositories;
using FCG_Users.Application.Shared.Results;
using FCG_Users.Application.Users.Interfaces;
using FCG_Users.Application.Users.Requests;
using FCG_Users.Application.Users.Responses;
using FCG_Users.Domain.Users.Entities;
using FCG_Users.Domain.Users.Enums;
using Fgc.Domain.Usuario.ObjetosDeValor;

namespace FCG_Users.Application.Users.Services
{
    public class AccountService(IAccountRepository repository, IJwtTokenService jwtService) : IAccountService
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
            
            return Result.Success(new AccountResponse(
                account.Id,
                account.Name,
                account.Password,
                account.Email,
                account.Profile,
                account.Active
            ));

        }
    }
}
