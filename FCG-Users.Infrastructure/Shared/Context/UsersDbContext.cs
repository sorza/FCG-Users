using FCG_Users.Domain.Users.Entities;
using FCG_Users.Infrastructure.Users.Mappings;
using Microsoft.EntityFrameworkCore;

namespace FCG_Users.Infrastructure.Shared.Context
{
    public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountMap());
        }
    }
}
