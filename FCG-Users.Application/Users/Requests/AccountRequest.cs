namespace FCG_Users.Application.Users.Requests
{
    public sealed record AccountRequest(string Name, string Password, string Email);
}
