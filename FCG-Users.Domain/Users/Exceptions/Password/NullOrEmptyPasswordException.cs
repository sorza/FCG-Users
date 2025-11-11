namespace FCG_Users.Domain.Users.Exceptions.Password
{
    public class NullOrEmptyPasswordException(string message) : Exception(message);
}
