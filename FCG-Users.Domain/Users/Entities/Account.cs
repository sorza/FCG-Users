using FCG.Shared.Contracts.ClassDefinition;
using FCG_Users.Domain.Users.Enums;
using FCG_Users.Domain.Users.Exceptions;
using FCG_Users.Domain.Users.Exceptions.Account;
using Fgc.Domain.Usuario.ObjetosDeValor;

namespace FCG_Users.Domain.Users.Entities
{
    public class Account : Entity
    {
        #region Construtors
        private Account() : base(Guid.NewGuid())
        {

        }

        private Account(Guid id, string name, Password password, Email email, EProfileType profile) : base(id)
        {
            Name = name;
            Password = password;
            Email = email;
            Profile = profile;
        }

        private Account(Guid id, string name, string password, Email email, EProfileType profile) : base(id)
        {
            Name = name;
            Password = Password.CreateFromHash(password);
            Email = email;
            Profile = profile;
        }

        #endregion

        #region Properties
        public string Name { get; private set; } = string.Empty;
        public Password Password { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public EProfileType Profile { get; private set; }
        public bool Active { get; private set; } = true;

        #endregion

        #region Factory Method

        public static Account Create(string name, string password, string email, EProfileType profile)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new NullOrEmptyNameException(ErrorMessage.Account.NullOrEmpty);

            if (!Enum.IsDefined(typeof(EProfileType), profile))
                throw new InvalidProfileException(ErrorMessage.Account.InvalidProfileType);

            var senha_result = Password.Create(password);
            var email_result = Email.Create(email);

            return new Account(Guid.NewGuid(), name, senha_result, email_result, profile);
        }

        public static Account Create(string name, string passwordHash, string email, EProfileType profile, bool active)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new NullOrEmptyNameException(ErrorMessage.Account.NullOrEmpty);

            if (!Enum.IsDefined(typeof(EProfileType), profile))
                throw new InvalidProfileException(ErrorMessage.Account.InvalidProfileType);
           
            var email_result = Email.Create(email);

            var account = new Account(Guid.NewGuid(), name, passwordHash, email_result, profile);
            account.Active = active;

            return account;
        }

        #endregion
    }
}
