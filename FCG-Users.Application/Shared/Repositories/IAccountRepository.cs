using FCG_Users.Domain.Users.Entities;
using Fgc.Domain.Usuario.ObjetosDeValor;

namespace FCG_Users.Application.Shared.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account?> Auth(Email email, CancellationToken cancellationToken);
        Task<bool> Exists(string email, CancellationToken cancellationToken = default);
    }
}
