using FCG.Shared.Contracts.Events.Domain.Users;
using FCG.Shared.Contracts.Interfaces;
using FCG.Shared.Contracts.Results;
using FCG_Users.Application.Shared.Interfaces;
using FCG_Users.Application.Shared.Repositories;
using FCG_Users.Application.Users.Requests;
using FCG_Users.Application.Users.Responses;
using FCG_Users.Domain.Users.Entities;
using FCG_Users.Domain.Users.Enums;
using Fgc.Domain.Usuario.ObjetosDeValor;
using FluentValidation;

namespace FCG_Users.Application.Users.Services
{
    public class AccountService(IAccountRepository repository, IJwtTokenService jwtService, IEventPublisher publisher, IValidator<AccountRequest> validator, IEventStore eventStore) : IAccountService
    {
        public async Task<Result<AuthResponse>> AuthAsync(AuthRequest request, string ip, string device, string correlationId, CancellationToken cancellationToken = default)
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

            var evt = new UserLoginEvent(conta.Id.ToString(), conta.Name, ip, device);

            var existingEvents = await eventStore.GetEventsAsync(conta.Id.ToString());
            var currentVersion = existingEvents.Count;
            
            await eventStore.AppendAsync(conta.Id.ToString(), evt, currentVersion, correlationId);
            await publisher.PublishAsync(evt, "UserLogin", correlationId);

            var response = new AuthResponse(tokenInfo.Token, tokenInfo.ExpiresAt);

            return Result.Success(response);

        }

        public async Task<Result<AccountResponse>> CreateAccountAsync(AccountRequest request, string correlationId, CancellationToken cancellationToken = default)
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
                return Result.Failure<AccountResponse>(new Error("400", string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

            var userExists = await repository.Exists(request.Email, cancellationToken);

            if (userExists)
                return Result.Failure<AccountResponse>(new Error("409", "Este usuário já está cadastrado."));

            var account = Account.Create(request.Name, request.Password, request.Email, EProfileType.Common);

            var evt = new UserCreatedEvent(account.Id.ToString(), account.Name,account.Password, account.Email, account.Profile.ToString(), account.Active);

            await eventStore.AppendAsync(account.Id.ToString(), evt, 0, correlationId);
            await publisher.PublishAsync(evt, "UserCreated", correlationId);

            return Result.Success(new AccountResponse(
                account.Id,
                account.Name,
                account.Password,
                account.Email,
                account.Profile,
                account.Active
            ));

        }

        public async Task<Result<IEnumerable<AccountResponse>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAllAsync(); 

            var users = result.Select(user => new AccountResponse(
                user.Id,
                user.Name,
                user.Password,
                user.Email,
                user.Profile,
                user.Active
                ));

            return Result.Success(users);
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

        public async Task<Result> RemoveUserAsync(Guid id, string correlationId, CancellationToken cancellationToken = default)
        {
            var usuario = await repository.GetByIdAsync(id);

            if (usuario is null)
                return Result.Failure(new Error("404", "Usuário não encontrado"));
            
            var evt = new UserDeletedEvent(usuario.Id.ToString(), usuario.Email);

            var existingEvents = await eventStore.GetEventsAsync(evt.AggregateId);
            var currentVersion = existingEvents.Count;
            
            await eventStore.AppendAsync(evt.AggregateId, evt, currentVersion, correlationId);
            await publisher.PublishAsync(evt, "UserDeleted", correlationId);

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
