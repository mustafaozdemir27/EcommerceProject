// ProductService.API/Program.cs
using Common.Infrastructure.Data; // IUnitOfWork için
using FluentValidation; // AddValidatorsFromAssemblyContaining için
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Features.Products.Commands.CreateProduct; // Assembly bulmak için
using ProductService.Application.Mappings; // ProductProfile için
using ProductService.Domain.Repositories; // IProductRepository için
using ProductService.Infrastructure.Persistence; // ProductDbContext için
using ProductService.Infrastructure.Persistence.Repositories; // ProductRepository için
// using ProductService.API.Middleware; // Henüz ProductService için özel bir middleware eklemedik

var builder = WebApplication.CreateBuilder(args);

// 1. Servisleri container'a ekleme

// DbContext'i ve IUnitOfWork'ü ekle
builder.Services.AddDbContext<ProductDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductServiceDb"));
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information);
        options.EnableSensitiveDataLogging();
    }
});
builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ProductDbContext>());

// Repository'leri ekle
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// MediatR'ı ekle (ProductService.Application katmanındaki handler'ları bulacak)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly));

// AutoMapper'ı ekle (ProductService.Application katmanındaki profilleri bulacak)
builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);

// FluentValidation'ı ekle (ProductService.Application katmanındaki validator'ları bulacak)
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>(ServiceLifetime.Scoped);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Product Service API",
        Description = "An ASP.NET Core Web API for managing Products"
    });
    // ProductService.API JWT doğrulaması yapacaksa, buraya Swagger için JWT ayarları eklenebilir
    // (tıpkı UserService.API ve ApiGateway'deki gibi).
    // Şimdilik, ProductService endpoint'lerinin Ocelot tarafından korunacağını
    // ve ProductService'in kendisinin token doğrulaması yapmayacağını varsayabiliriz
    // (veya yapacağını varsayıp JWT auth'u buraya da ekleyebiliriz).
    // BASİTLİK ADINA ŞİMDİLİK ProductService.API'ye JWT AUTH EKLEMEYELİM.
    // Ocelot'un bu servise gelen istekleri zaten doğrulamış olacağını veya
    // bazı endpoint'lerin herkese açık olacağını varsayalım.
});

var app = builder.Build();

// 2. HTTP request pipeline'ını yapılandırma
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API V1");
    });
    app.UseDeveloperExceptionPage();
}

// app.UseMiddleware<ExceptionHandlingMiddleware>(); // ProductService için de benzer bir middleware eklenebilir.
// Veya Common bir middleware kullanılabilir. Şimdilik eklemiyoruz.

// app.UseHttpsRedirection();
app.UseRouting();
// app.UseAuthentication(); // Şimdilik ProductService.API'de JWT auth middleware'i yok
// app.UseAuthorization();
app.MapControllers();
app.Run();