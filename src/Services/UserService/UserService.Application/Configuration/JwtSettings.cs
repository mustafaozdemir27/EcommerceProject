// UserService.Application/Configuration/JwtSettings.cs
namespace UserService.Application.Configuration
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings"; // appsettings.json'daki bölüm adı

        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
    }
}