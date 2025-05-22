// UserService.Application/Features/Users/Queries/GetUserById/GetUserByIdQueryHandler.cs
using AutoMapper; // IMapper için
using MediatR;
using UserService.Application.Features.Users.Dtos; // UserDto için
using UserService.Domain.Repositories; // IUserRepository için
// using UserService.Application.Contracts.Persistence; // Eğer IUserRepository burada olsaydı

namespace UserService.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Repository üzerinden kullanıcıyı ID ile getir
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            // 2. Kullanıcı bulunamadıysa null dön
            if (user == null)
            {
                return null;
                // Alternatif olarak, burada özel bir NotFoundException fırlatılabilir
                // ve bu exception API katmanında yakalanarak 404 Not Found yanıtı döndürülebilir.
                // throw new NotFoundException(nameof(User), request.Id);
            }

            // 3. User entity'sini UserDto'ya maple
            var userDto = _mapper.Map<UserDto>(user);

            // 4. DTO'yu döndür
            return userDto;
        }
    }
}