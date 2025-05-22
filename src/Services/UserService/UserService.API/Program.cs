// UserService.API/Program.cs
using Common.Infrastructure.Data; // IUnitOfWork için
using FluentValidation; // AddValidatorsFromAssemblyContaining için (veya ilgili extension metodun namespace'i)
using Microsoft.EntityFrameworkCore;
using UserService.API.Middleware;
using UserService.Application.Features.Users.Commands.RegisterUser; // RegisterUserCommandHandler ve RegisterUserCommandValidator (assembly bulmak için)
using UserService.Application.Mappings; // UserProfile (AutoMapper profili) için
using UserService.Domain.Repositories; // IUserRepository için
using UserService.Infrastructure.Persistence.Repositories; // UserRepository için

var builder = WebApplication.CreateBuilder(args);

// 1. Servisleri container'a ekleme (Dependency Injection)

// DbContext'i ve IUnitOfWork'ü ekle
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserServiceDb"));
    // Geliştirme ortamında EF Core tarafından çalıştırılan SQL komutlarını ve parametreleri görmek faydalıdır.
    // Üretimde LogLevel.Information veya LogLevel.Warning daha uygun olabilir.
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information); // Temel loglama
        options.EnableSensitiveDataLogging(); // Sadece geliştirme ortamında!
    }
});

// IUnitOfWork istendiğinde, AddDbContext ile kaydedilen UserDbContext örneğinin kullanılmasını sağla
builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<UserDbContext>());

// Repository'leri ekle
builder.Services.AddScoped<IUserRepository, UserRepository>();

// MediatR'ı ekle (Application katmanındaki handler'ları bulacak)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));

// AutoMapper'ı ekle (Application katmanındaki profilleri bulacak)
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

// FluentValidation'ı ekle (Application katmanındaki validator'ları bulacak)
// FluentValidation.AspNetCore paketindeki eski metotlar (AddFluentValidationAutoValidation vb.)
// yerine FluentValidation.DependencyInjectionExtensions paketindeki bu metodu kullanıyoruz.
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>(ServiceLifetime.Scoped);


builder.Services.AddControllers(); // Controller'ları kullanacağımızı belirtiyoruz.

// Swagger/OpenAPI yapılandırması
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "User Service API",
        Description = "An ASP.NET Core Web API for managing Users"
    });
});


var app = builder.Build();

// 2. HTTP request pipeline'ını yapılandırma

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API V1");
        // options.RoutePrefix = string.Empty; // Swagger UI'ı ana sayfada göstermek için
    });
    app.UseDeveloperExceptionPage(); // Geliştirme ortamında detaylı hata sayfası
}
else
{
    // Üretim ortamı için genel bir hata handler'ı eklenebilir.
    // app.UseExceptionHandler("/Error");
    // app.UseHsts(); // HTTPS Strict Transport Security
}

// Exception Handling Middleware'imizi pipeline'a ekle
// Bu, UseRouting gibi diğer middleware'lerden önce gelirse daha geniş kapsamlı hata yakalama sağlar.
// Ancak UseDeveloperExceptionPage'den sonra gelmesi, geliştirme ortamında öncelikle onun çalışmasını sağlar.
// Genellikle pipeline'ın başlarında, ancak statik dosyalar veya routing gibi temel şeylerden sonra yer alır.
// UseDeveloperExceptionPage'i sadece geliştirme için tutuyorsak, bu middleware'i onun dışına koyabiliriz.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// app.UseHttpsRedirection(); // HTTPS yönlendirmesi (API Gateway arkasında çalışıyorsa veya Kestrel'de SSL yoksa kapatılabilir)

app.UseRouting(); // Routing middleware'i

// app.UseAuthentication(); // Kimlik doğrulama eklendiğinde bu satır aktif edilecek
app.UseAuthorization(); // Yetkilendirme middleware'i

app.MapControllers(); // Controller endpoint'lerini eşle

app.Run();