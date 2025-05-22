// UserService.API/Controllers/UsersController.cs
using MediatR; // IMediator için
using Microsoft.AspNetCore.Mvc; // ApiController, ControllerBase, Route, HttpGet, HttpPost vb. için
using UserService.Application.Features.Users.Commands.RegisterUser; // RegisterUserCommand için
using UserService.Application.Features.Users.Commands.UpdateUserProfile; // UpdateUserProfileCommand için
using UserService.Application.Features.Users.Dtos; // UserDto için (dönüş tipi olarak)
using UserService.Application.Features.Users.Queries.GetUserById;  // GetUserByIdQuery için
using UserService.Application.Features.Users.Queries.ListUsers; // ListUsersQuery için

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/users
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // POST api/users
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command)
        {
            // FluentValidation.AspNetCore entegrasyonu sayesinde, RegisterUserCommand
            // otomatik olarak RegisterUserCommandValidator ile doğrulanacaktır.
            // Eğer doğrulama başarısız olursa, ASP.NET Core pipeline'ı otomatik olarak
            // 400 Bad Request yanıtı dönecektir (ModelState.IsValid false olur).

            var userId = await _mediator.Send(command);

            // Yeni oluşturulan kaynağın URI'sini ve ID'sini döndür.
            // GetUserById action'ı için bir isim verip onu kullanabiliriz (nameof(GetUserById)).
            return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { id = userId });
        }

        // GET api/users/{id}
        [HttpGet("{id:guid}", Name = "GetUserById")] // Name = "GetUserById" CreatedAtAction için kullanılır
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var query = new GetUserByIdQuery(id);
            var userDto = await _mediator.Send(query);

            if (userDto == null)
            {
                return NotFound(); // 404 Not Found
            }

            return Ok(userDto); // 200 OK
        }

        // PUT api/users/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Başarılı güncelleme için
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Validasyon hataları için
        [ProducesResponseType(StatusCodes.Status404NotFound)]   // Kullanıcı bulunamazsa
        public async Task<IActionResult> UpdateUserProfile(Guid id, [FromBody] UpdateUserProfileCommand command)
        {
            if (command == null) // Body boş gelirse veya deserialize edilemezse
            {
                return BadRequest("İstek gövdesi boş olamaz veya geçersiz formatta.");
            }

            // Route'tan gelen ID'yi komut nesnesine ata.
            // Bu, komutun hem ID'yi hem de body'den gelen diğer verileri içermesini sağlar.
            command.Id = id;

            // MediatR'a komutu gönder.
            // FluentValidation (AddValidatorsFromAssemblyContaining ile kaydedilen)
            // ve [ApiController] attribute'ü sayesinde 'command' nesnesi
            // otomatik olarak UpdateUserProfileCommandValidator ile doğrulanacaktır.
            // Eğer doğrulama başarısız olursa, pipeline otomatik 400 Bad Request döner.
            await _mediator.Send(command);

            return NoContent(); // 204 No Content - Başarılı güncelleme, yanıt gövdesi yok.
        }

        // GET api/users
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsers()
        {
            var query = new ListUsersQuery(); // Sorgu nesnesini oluştur
            var users = await _mediator.Send(query); // MediatR ile sorguyu gönder

            return Ok(users); // 200 OK yanıtıyla birlikte kullanıcı listesini (UserDto listesi) döndür
        }
    }
}