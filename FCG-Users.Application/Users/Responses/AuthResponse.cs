namespace FCG_Users.Application.Users.Responses
{
    public sealed record AuthResponse(string AccessToken, DateTime ExpiresAt); 
}
