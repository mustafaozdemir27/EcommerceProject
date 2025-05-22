// UserService.API/Middleware/ExceptionHandlingMiddleware.cs
using Microsoft.AspNetCore.Mvc; // ProblemDetails, ValidationProblemDetails için
using System.Net; // HttpStatusCode için
using System.Text.Json; // JsonSerializer için
using UserService.Application.Exceptions; // DuplicateValueException için

namespace UserService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env; // Geliştirme ortamı kontrolü için

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env) // IHostEnvironment'ı enjekte et
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Bir sonraki middleware'i veya endpoint'i çağır
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Hata oluşursa, logla ve uygun HTTP yanıtını oluştur
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Beklenmedik bir hata oluştu: {ErrorMessage}", exception.Message);

            HttpStatusCode statusCode;
            ProblemDetails problemDetails; // RFC 7807 standardına uygun hata detayı

            switch (exception)
            {
                case DuplicateValueException duplicateValueEx:
                    statusCode = HttpStatusCode.Conflict; // 409 Conflict (veya 400 Bad Request)
                    problemDetails = new ProblemDetails
                    {
                        Title = "Çakışan Değer", // "Duplicate Value"
                        Status = (int)statusCode,
                        Detail = duplicateValueEx.Message,
                        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8" // 409 Conflict için RFC
                    };
                    break;

                case FluentValidation.ValidationException validationEx:
                    statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
                    // FluentValidation'dan gelen hataları daha yapısal bir şekilde sunmak için ValidationProblemDetails kullanılır.
                    var errors = validationEx.Errors
                        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                        .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

                    problemDetails = new ValidationProblemDetails(errors)
                    {
                        Title = "Bir veya daha fazla doğrulama hatası oluştu.", // "One or more validation errors occurred."
                        Status = (int)statusCode,
                        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1" // 400 Bad Request için RFC
                    };
                    break;

                // Buraya diğer özel exception türleriniz için case'ler ekleyebilirsiniz
                // Örnek:
                // case NotFoundException notFoundEx:
                //     statusCode = HttpStatusCode.NotFound; // 404 Not Found
                //     problemDetails = new ProblemDetails { ... };
                //     break;

                default: // Bilinmeyen veya genel Exception'lar için
                    statusCode = HttpStatusCode.InternalServerError; // 500 Internal Server Error
                    problemDetails = new ProblemDetails
                    {
                        Title = "Beklenmedik bir sunucu hatası oluştu.", // "An unexpected server error occurred."
                        Status = (int)statusCode,
                        Detail = _env.IsDevelopment() ? exception.ToString() : "İç sunucu hatası. Lütfen daha sonra tekrar deneyin.",
                        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1" // 500 Internal Server Error için RFC
                    };
                    break;
            }

            context.Response.ContentType = "application/problem+json"; // Yanıt tipini belirt
            context.Response.StatusCode = (int)statusCode;

            // ProblemDetails nesnesini JSON olarak yanıt gövdesine yaz
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
        }
    }
}