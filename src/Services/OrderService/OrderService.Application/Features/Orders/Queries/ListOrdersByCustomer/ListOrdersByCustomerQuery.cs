// OrderService.Application/Features/Orders/Queries/ListOrdersByCustomer/ListOrdersByCustomerQuery.cs
using MediatR;
using OrderService.Application.Features.Orders.Dtos;

namespace OrderService.Application.Features.Orders.Queries.ListOrdersByCustomer
{
    public class ListOrdersByCustomerQuery : IRequest<IEnumerable<OrderDto>>
    {
        public Guid CustomerId { get; }

        public ListOrdersByCustomerQuery(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                throw new ArgumentException("Müşteri ID'si boş olamaz.", nameof(customerId));
            }
            CustomerId = customerId;
        }
        // İleride sayfalama parametreleri eklenebilir.
    }
}