using FCG_Users.Domain.Shared;
using FCG_Users.Domain.Users.Exceptions;
using FCG_Users.Domain.Users.Exceptions.Password;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Fgc.Domain.Usuario.ObjetosDeValor
{
    public sealed partial record Password : ValueObject
    {
        #region Constants

        public const int MaxLength = 50;
        public const int MinLength = 8;
        public const string Pattern = @"^(?=.{8,50}$)(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).*$";

        #endregion

        #region Properties     
        public string Hash { get; private set; } = string.Empty;
        #endregion

        #region Construtors

        private Password()
        {

        }

        private Password(string hash) => Hash = hash;

        #endregion

        #region Factory Method
        public static Password Create(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password == string.Empty)
                throw new NullOrEmptyPasswordException(ErrorMessage.Password.NullOrEmpty);

            if (password.Length is < MinLength or > MaxLength)
                throw new InvalidPasswordException(ErrorMessage.Password.Invalid);

            if (!SenhaRegex().IsMatch(password))
                throw new InvalidPasswordException(ErrorMessage.Password.Invalid);

            var hasher = new Rfc2898DeriveBytes(
                                password,
                                saltSize: 16,
                                iterations: 100_000,
                                HashAlgorithmName.SHA256);

            var salt = hasher.Salt;
            var hashed = hasher.GetBytes(32);
            var result = Convert.ToBase64String(salt.Concat(hashed).ToArray());

            return new Password(result);

        }

        #endregion

        #region Methods
        public bool Verify(string password)
        {
            var data = Convert.FromBase64String(Hash);
            var salt = data[..16];
            var storedHash = data[16..];
           
            using var derive = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations: 100_000,
                HashAlgorithmName.SHA256);

            var computedHash = derive.GetBytes(32);
            
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);

        }
        #endregion

        #region Operators

        public static implicit operator string(Password senha) => senha.ToString();


        #endregion

        #region Overrides

        public override string ToString() => Hash;

        #endregion

        #region Others
        [GeneratedRegex(Pattern)]
        private static partial Regex SenhaRegex();

        #endregion
    }
}
