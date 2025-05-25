// OrderService.Application/Features/Orders/Queries/ListOrdersByCustomer/ListOrdersByCustomerQueryHandler.cs
using AutoMapper;
using MediatR;
using OrderService.Application.Features.Orders.Dtos;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Features.Orders.Queries.ListOrdersByCustomer
{
    public class ListOrdersByCustomerQueryHandler : IRequestHandler<ListOrdersByCustomerQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public ListOrdersByCustomerQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new System.ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<OrderDto>> Handle(ListOrdersByCustomerQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}