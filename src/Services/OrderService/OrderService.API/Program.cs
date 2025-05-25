// OrderService.API/Program.cs
using Common.Infrastructure.Data; // IUnitOfWork için
using FluentValidation; // AddValidatorsFromAssemblyContaining için
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Features.Orders.Commands.CreateOrder; // Assembly bulmak için
using OrderService.Application.Mappings; // OrderProfile için
using OrderService.Domain.Repositories; // IOrderRepository için
using OrderService.Infrastructure.Persistence; // OrderDbContext için
using OrderService.Infrastructure.Persistence.Repositories; // OrderRepository için
// using OrderService.API.Middleware; // Henüz OrderService için özel bir middleware eklemedik

var builder = WebApplication.CreateBuilder(args);

// 1. Servisleri container'a ekleme

// DbContext'i ve IUnitOfWork'ü ekle
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderServiceDb"));
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information);
        options.EnableSensitiveDataLogging();
    }
});
// IUnitOfWork kaydını doğru şekilde yapıyoruz:
builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<OrderDbContext>());

// Repository'leri ekle
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// MediatR'ı ekle (OrderService.Application katmanındaki handler'ları bulacak)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly));

// AutoMapper'ı ekle (OrderService.Application katmanındaki profilleri bulacak)
builder.Services.AddAutoMapper(typeof(OrderProfile).Assembly);

// FluentValidation'ı ekle (OrderService.Application katmanındaki validator'ları bulacak)
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>(ServiceLifetime.Scoped);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Order Service API",
        Description = "An ASP.NET Core Web API for managing Orders"
    });
    // Eğer OrderService.API endpoint'leri JWT ile korunacaksa,
    // Swagger için JWT ayarları buraya eklenebilir (UserService.API ve ApiGateway'deki gibi).
    // Şimdilik Ocelot'un bu servise gelen istekleri zaten doğrulamış olacağını
    // veya bazı endpoint'lerin herkese açık olacağını varsayalım.
});

var app = builder.Build();

// 2. HTTP request pipeline'ını yapılandırma
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API V1");
    });
    app.UseDeveloperExceptionPage();
}

// app.UseMiddleware<ExceptionHandlingMiddleware>(); // OrderService için de benzer bir middleware eklenebilir.

// app.UseHttpsRedirection();
app.UseRouting();

// Eğer OrderService.API kendi içinde token doğrulayacaksa bu satırlar aktif edilmeli:
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();
app.Run();