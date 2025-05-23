// UserService.Application/Contracts/Security/GeneratedToken.cs
namespace UserService.Application.Contracts.Security
{
    public class GeneratedToken
    {
        public string Token { get; }
        public DateTime ExpiresAt { get; }

        public GeneratedToken(string token, DateTime expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
        }
    }
}