// UserService.API/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // LogLevel için (EF Core loglamasında)
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // OpenApiInfo ve OpenApiSecurityScheme için
using System.Text;
using UserService.Application.Configuration; // JwtSettings için
using UserService.Application.Contracts.Security; // IPasswordHasherService, IJwtTokenGenerator için
using UserService.Application.Features.Users.Commands.RegisterUser; // RegisterUserCommandHandler ve RegisterUserCommandValidator (assembly bulmak için)
using UserService.Application.Mappings; // UserProfile (AutoMapper profili) için
using UserService.Domain.Repositories; // IUserRepository için
using UserService.Infrastructure.Persistence; // UserDbContext için
using UserService.Infrastructure.Persistence.Repositories; // UserRepository için
using UserService.Infrastructure.Security; // BCryptPasswordHasherService, JwtTokenGenerator için
using Common.Infrastructure.Data; // IUnitOfWork için
using FluentValidation; // AddValidatorsFromAssemblyContaining için
using UserService.API.Middleware; // ExceptionHandlingMiddleware için

var builder = WebApplication.CreateBuilder(args);

// === DEBUG SATIRLARI BAŞLANGIÇ ===
Console.WriteLine("--- UserService.API Program.cs (DEBUG START) ---");
Console.WriteLine($"UserService.API Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"UserService.API ContentRootPath: {builder.Environment.ContentRootPath}");

// Doğrudan configuration üzerinden Key ve Issuer okuma denemesi
var directJwtKey = builder.Configuration["JwtSettings:Key"];
Console.WriteLine($"DEBUG (UserService.API): Configuration['JwtSettings:Key'] = '{directJwtKey}'");

var directJwtIssuer = builder.Configuration["JwtSettings:Issuer"];
Console.WriteLine($"DEBUG (UserService.API): Configuration['JwtSettings:Issuer'] = '{directJwtIssuer}'");

// JwtSettings bölümünü kontrol etme
var jwtSettingsSectionForDebug = builder.Configuration.GetSection("JwtSettings"); // <-- Değişken burada tanımlanıyor

if (jwtSettingsSectionForDebug.Exists()) // <-- Tanımlanan değişken burada kullanılıyor
{
    Console.WriteLine("DEBUG (UserService.API): 'JwtSettings' bölümü yapılandırmada MEVCUT.");
    var keyFromSection = jwtSettingsSectionForDebug["Key"]; // <-- Tanımlanan değişken burada kullanılıyor
    Console.WriteLine($"DEBUG (UserService.API): 'JwtSettings:Key' from GetSection = '{keyFromSection}'");
    var issuerFromSection = jwtSettingsSectionForDebug["Issuer"]; // Ek kontrol
    Console.WriteLine($"DEBUG (UserService.API): 'JwtSettings:Issuer' from GetSection = '{issuerFromSection}'");
}
else
{
    Console.WriteLine("DEBUG (UserService.API): 'JwtSettings' bölümü yapılandırmada MEVCUT DEĞİL.");
}
Console.WriteLine("--- UserService.API Program.cs (DEBUG END) ---");
// === DEBUG SATIRLARI SONU ===

// 1. Servisleri container'a ekleme (Dependency Injection)

// --- Temel Servisler ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- Swagger/OpenAPI Yapılandırması ---
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "User Service API",
        Description = "An ASP.NET Core Web API for managing Users"
    });

    // JWT Authentication'ı Swagger UI'a entegre etme (Authorize butonu için)
    // SecuritySchemeType.Http ve scheme: "bearer" kullanarak daha iyi entegrasyon
    options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme // Tanım için bir isim (örn: BearerAuth)
    {
        Name = "Authorization", // Header adı
        Type = SecuritySchemeType.Http, // HTTP tabanlı kimlik doğrulama
        Scheme = "bearer", // Kullanılacak şema (küçük harfle "bearer")
        BearerFormat = "JWT", // Token formatı
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                      "Enter your token in the text input below (without the 'Bearer ' prefix). \r\n\r\n" +
                      "Example: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BearerAuth" // AddSecurityDefinition'da verdiğimiz ID ile eşleşmeli
                }
            },
            Array.Empty<string>()
        }
    });
});

// --- Uygulama Yapılandırmaları ---
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// --- Veritabanı ve UnitOfWork ---
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserServiceDb"));
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information);
        options.EnableSensitiveDataLogging();
    }
});
builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<UserDbContext>());

// --- Repository'ler ---
builder.Services.AddScoped<IUserRepository, UserRepository>();

// --- Uygulama Servisleri ve Yardımcıları ---
builder.Services.AddSingleton<IPasswordHasherService, BCryptPasswordHasherService>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

// --- MediatR ---
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));

// --- AutoMapper ---
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

// --- FluentValidation ---
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>(ServiceLifetime.Scoped);

// --- Authentication (JWT Bearer) ---
var jwtSettingsFromConfig = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
if (jwtSettingsFromConfig == null || string.IsNullOrEmpty(jwtSettingsFromConfig.Key) ||
    string.IsNullOrEmpty(jwtSettingsFromConfig.Issuer) || string.IsNullOrEmpty(jwtSettingsFromConfig.Audience))
{
    // Bu kontrol, uygulamanın başlangıcında kritik JWT ayarlarının eksik olup olmadığını doğrular.
    // Eğer eksikse, ArgumentNullException yerine InvalidOperationException fırlatmak daha uygun olabilir,
    // çünkü bu bir programlama hatasından ziyade eksik yapılandırma sorunudur.
    throw new InvalidOperationException($"JWT settings ('{JwtSettings.SectionName}') section with Key, Issuer, and Audience must be configured in appsettings.json for JWT Authentication setup.");
}
var keyBytes = Encoding.UTF8.GetBytes(jwtSettingsFromConfig.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettingsFromConfig.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettingsFromConfig.Audience,

        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuerSigningKey = true,

        ClockSkew = TimeSpan.Zero
    };
});


// 2. HTTP request pipeline'ını yapılandırma
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API V1");
    });
    app.UseDeveloperExceptionPage();
}
else
{
    // app.UseExceptionHandler("/Error");
    // app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); // Kimlik doğrulama
app.UseAuthorization();  // Yetkilendirme

app.MapControllers();

app.Run();