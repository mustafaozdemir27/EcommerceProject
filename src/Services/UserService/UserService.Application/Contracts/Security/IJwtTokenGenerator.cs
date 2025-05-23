// UserService.Application/Contracts/Security/IJwtTokenGenerator.cs
using UserService.Domain;         // User entity için (veya kullanıcı bilgilerini taşıyan bir DTO)

namespace UserService.Application.Contracts.Security
{
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Verilen kullanıcı için bir JWT oluşturur.
        /// </summary>
        /// <param name="user">Token oluşturulacak kullanıcı entity'si.</param>
        /// <param name="roles">Kullanıcının rolleri (isteğe bağlı).</param>
        /// <returns>Oluşturulan JWT string'i.</returns>
        GeneratedToken GenerateToken(User user, IEnumerable<string>? roles = null);
    }
}