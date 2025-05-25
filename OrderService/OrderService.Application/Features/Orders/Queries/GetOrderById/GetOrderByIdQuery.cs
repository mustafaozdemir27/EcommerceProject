// OrderService.Application/Features/Orders/Queries/GetOrderById/GetOrderByIdQuery.cs
using MediatR;
using OrderService.Application.Features.Orders.Dtos; // OrderDto için

namespace OrderService.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDto?> // OrderDto? : Sipariş bulunamazsa null dönebilir
    {
        public Guid Id { get; }

        public GetOrderByIdQuery(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Sipariş ID'si boş olamaz.", nameof(id));
            }
            Id = id;
        }
    }
}