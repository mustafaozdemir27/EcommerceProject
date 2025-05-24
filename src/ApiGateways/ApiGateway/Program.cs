// ApiGateway/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer; // Bu using'i ekleyin
using Microsoft.IdentityModel.Tokens;             // Bu using'i ekleyin
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;                                  // Bu using'i ekleyin
                                                    // using UserService.Application.Configuration; // JwtSettings'i doğrudan okuyacaksak bu gerekmeyebilir,
                                                    // builder.Configuration'ı kullanacağız.

var builder = WebApplication.CreateBuilder(args);

// ocelot.json'ı yapılandırmaya ekle
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// --- JWT Ayarlarını Yükleme (Ocelot'un Authentication'ı için) ---
// Ocelot, kendi authentication provider'ını yapılandırırken bu ayarlara ihtiyaç duyacak.
// Bu ayarların UserService.API'deki appsettings.json'daki JwtSettings ile aynı olması gerekir
// ki Ocelot, UserService'in ürettiği token'ları doğrulayabilsin.
// Bu ayarları doğrudan ocelot.json içine de koyabiliriz veya appsettings.json'dan okuyabiliriz.
// Şimdilik appsettings.json'dan okuyalım (ApiGateway projesinin kendi appsettings.json'ı).
// Bu nedenle ApiGateway projesinin appsettings.Development.json dosyasına da
// UserService.API'deki gibi bir JwtSettings bölümü eklememiz gerekecek.
// VEYA bu ayarları doğrudan ocelot.json'a gömebiliriz.
// En temizi, bu ayarları ApiGateway'in kendi appsettings'inden okumasıdır.

// **ÖNEMLİ:** ApiGateway'in appsettings.Development.json dosyasına,
// UserService.API'deki JwtSettings bölümünün AYNISINI ekleyin.
// Özellikle "Key", "Issuer" ve "Audience" aynı olmalı.
var jwtKeyFromConfig = builder.Configuration["JwtSettings:Key"];
var jwtIssuerFromConfig = builder.Configuration["JwtSettings:Issuer"];
var jwtAudienceFromConfig = builder.Configuration["JwtSettings:Audience"];

if (string.IsNullOrEmpty(jwtKeyFromConfig) || string.IsNullOrEmpty(jwtIssuerFromConfig) || string.IsNullOrEmpty(jwtAudienceFromConfig))
{
    throw new InvalidOperationException("JWT settings (Key, Issuer, Audience) for Ocelot authentication must be configured in ApiGateway's appsettings.json.");
}
var keyBytes = Encoding.UTF8.GetBytes(jwtKeyFromConfig);

// --- Ocelot Authentication Servislerini Ekleme ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("GatewayAuthenticationScheme", options => // Ocelot için özel bir scheme adı verelim
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = false; // Ocelot token'ı sadece doğrular, saklamasına gerek yok
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuerFromConfig,

        ValidateAudience = true,
        ValidAudience = jwtAudienceFromConfig,

        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Ocelot servislerini DI'a ekle
builder.Services.AddOcelot(builder.Configuration); // Veya sadece AddOcelot();

var app = builder.Build();

// Ocelot middleware'ini pipeline'a ekle
// Authentication middleware'i Ocelot'tan ÖNCE gelmeli ki Ocelot onu kullanabilsin.
// Düzeltme: Ocelot kendi authentication'ını yönettiği için,
// app.UseAuthentication() genellikle Ocelot'tan sonra veya Ocelot'un
// kendi pipeline'ı içinde ele alınır. Ocelot dokümantasyonuna göre,
// AddJwtBearer'ı yukarıdaki gibi tanımlamak ve ocelot.json'da belirtmek yeterlidir.
// app.UseAuthentication(); // Bu satır burada gerekli olmayabilir, ocelot.json'daki AuthenticationProviderKey ile yönetilir.

await app.UseOcelot();

app.Run();