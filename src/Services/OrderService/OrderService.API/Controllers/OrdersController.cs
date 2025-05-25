// OrderService.API/Controllers/OrdersController.cs
using MediatR; // IMediator için
using Microsoft.AspNetCore.Mvc; // ApiController, ControllerBase, Route, HttpGet, HttpPost vb. için
using OrderService.Application.Features.Orders.Commands.CreateOrder; // CreateOrderCommand için
using OrderService.Application.Features.Orders.Dtos; // OrderDto için
using OrderService.Application.Features.Orders.Queries.GetOrderById;    // GetOrderByIdQuery için
using OrderService.Application.Features.Orders.Queries.ListOrdersByCustomer; // ListOrdersByCustomerQuery için
// using Microsoft.AspNetCore.Authorization; // Eğer endpoint'leri korumak istersek

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route: api/orders
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // POST api/orders
        // [Authorize] // Bu endpoint'i korumak isteyebiliriz (sadece login olmuş kullanıcılar sipariş verebilir)
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)] // Dönen tip Guid (yeni sipariş ID'si)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Eğer [Authorize] eklersek
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            // FluentValidation (AddValidatorsFromAssemblyContaining ile kaydedilen)
            // ve [ApiController] attribute'ü sayesinde 'command' nesnesi
            // otomatik olarak CreateOrderCommandValidator ile doğrulanacaktır.
            // Eğer doğrulama başarısız olursa, pipeline otomatik 400 Bad Request döner.

            var orderId = await _mediator.Send(command);

            // Yeni oluşturulan kaynağın ID'sini ve konumunu (GetOrderById endpoint'ine referansla) döndür.
            return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { id = orderId });
        }

        // GET api/orders/{id}
        // [Authorize] // Belki sadece siparişi veren kullanıcı veya admin görebilir
        [HttpGet("{id:guid}", Name = "GetOrderById")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Eğer [Authorize] eklersek
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var query = new GetOrderByIdQuery(id);
            var orderDto = await _mediator.Send(query);

            if (orderDto == null)
            {
                return NotFound(); // 404 Not Found
            }

            return Ok(orderDto); // 200 OK
        }

        // GET api/orders/customer/{customerId}
        // [Authorize] // Sadece ilgili müşteri veya admin kendi siparişlerini görebilir
        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Eğer [Authorize] eklersek
        // [ProducesResponseType(StatusCodes.Status404NotFound)] // Eğer müşteri ID'si için özel bir kontrol varsa
        public async Task<IActionResult> ListOrdersByCustomer(Guid customerId)
        {
            var query = new ListOrdersByCustomerQuery(customerId);
            var orders = await _mediator.Send(query);
            return Ok(orders); // Müşterinin hiç siparişi yoksa boş liste döner, bu bir hata değildir.
        }

        // İleride eklenecek diğer endpoint'ler (UpdateOrderStatus vb.) buraya gelecek.
    }
}