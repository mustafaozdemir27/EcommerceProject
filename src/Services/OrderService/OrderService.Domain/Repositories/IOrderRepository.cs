// OrderService.Domain/Repositories/IOrderRepository.cs
namespace OrderService.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        void Update(Order order);
        // Task DeleteAsync(Order order, CancellationToken cancellationToken = default); // Gerekirse
    }
}