using FCG_Users.Domain.Users.Entities;
using Fgc.Domain.Usuario.ObjetosDeValor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG_Users.Infrastructure.Users.Mappings
{
    public class AccountMap : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(x => x.Id)
                .HasName("PK_Account");

            builder.Property(x => x.Name)
                .HasColumnType("VARCHAR(150)")
                .IsRequired(true);

            builder.OwnsOne(x => x.Email, email =>
            {
                email.Property(x => x.Address)
                .HasColumnName("Email")
                .HasColumnType("VARCHAR")
                .HasMaxLength(Email.MaxLength)
                .IsRequired(true);
            });

            builder.OwnsOne(x => x.Password, senha =>
            {
                senha.Property(s => s.Hash)
                .HasColumnName("Password")
                .HasMaxLength(256)
                .IsRequired(true);
            });
        }
    }
}
