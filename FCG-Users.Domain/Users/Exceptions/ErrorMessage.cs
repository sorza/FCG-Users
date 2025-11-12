namespace FCG_Users.Domain.Users.Exceptions
{
    public class ErrorMessage
    {
        #region Properties
        public static EmailErrorMessages Email { get; } = new();
        public static PasswordErrorMessages Password { get; } = new();
        public static AccountErrorMessages Account { get; } = new();

        #endregion       

        public class EmailErrorMessages
        {
            public string NullOrEmpty { get; } = "O email deve ser informado";
            public string Invalid { get; } = "O email informado é inválido";
        }

        public class PasswordErrorMessages
        {
            public string NullOrEmpty { get; } = "A senha deve ser informada";
            public string Invalid { get; } = "A senha informada é inválida";
        }

        public class AccountErrorMessages
        {
            public string NullOrEmpty { get; } = "O nome deve ser informado";         
            public string InvalidProfileType { get; } = "Tipo de perfil inválido";
        }
    }
}
