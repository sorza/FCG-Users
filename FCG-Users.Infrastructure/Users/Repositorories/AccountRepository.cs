using FCG_Users.Domain.Users.Entities;
using FCG_Users.Infrastructure.Shared.Context;
using FCG_Users.Infrastructure.Shared.Repositories;
using FCG_Users.Application.Shared.Repositories;
using Fgc.Domain.Usuario.ObjetosDeValor;
using Microsoft.EntityFrameworkCore;

namespace FCG_Users.Infrastructure.Users.Repositorories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly UsersDbContext _context;
        public AccountRepository(UsersDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Exists(string email, CancellationToken cancellationToken = default)
         => await _context.Accounts
                .AsNoTracking()
                .AnyAsync(u => u.Email.Address == email);

        public async Task<Account?> Auth(Email email, CancellationToken cancellationToken)
         => await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Address == email.Address, cancellationToken);

    }
}
