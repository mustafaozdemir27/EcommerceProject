// OrderService.Application/Features/Orders/Queries/GetOrderById/GetOrderByIdQueryHandler.cs
using AutoMapper; // IMapper için
using MediatR;
using OrderService.Application.Features.Orders.Dtos; // OrderDto için
using OrderService.Domain.Repositories;             // IOrderRepository için
// using OrderService.Application.Exceptions;       // NotFoundException (ileride eklenebilir)

namespace OrderService.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new System.ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

            if (order == null)
            {
                return null; // Veya throw new NotFoundException("Order", request.Id);
            }

            // Order entity'sini OrderDto'ya maple.
            // OrderProfile'ımız OrderItems için de mapping içerdiği için bu direkt çalışacaktır.
            return _mapper.Map<OrderDto>(order);
        }
    }
}