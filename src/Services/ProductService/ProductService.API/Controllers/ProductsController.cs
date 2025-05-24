// ProductService.API/Controllers/ProductsController.cs
using MediatR; // IMediator için
using Microsoft.AspNetCore.Mvc; // ApiController, ControllerBase, Route, HttpGet, HttpPost vb. için
using ProductService.Application.Features.Products.Commands.CreateProduct; // CreateProductCommand için
using ProductService.Application.Features.Products.Dtos; // ProductDto için
using ProductService.Application.Features.Products.Queries.GetProductById; // GetProductByIdQuery için (ileride eklenecek)
using ProductService.Application.Features.Products.Queries.ListProducts;  // ListProductsQuery için

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route: api/products
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // POST api/products
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)] // Dönen tip Guid (yeni ürün ID'si)
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            // FluentValidation (AddValidatorsFromAssemblyContaining ile kaydedilen)
            // ve [ApiController] attribute'ü sayesinde 'command' nesnesi
            // otomatik olarak CreateProductCommandValidator ile doğrulanacaktır.
            // Eğer doğrulama başarısız olursa, pipeline otomatik 400 Bad Request döner.

            var productId = await _mediator.Send(command);

            // Yeni oluşturulan kaynağın ID'sini ve konumunu (GetProductById endpoint'ine referansla) döndür.
            // GetProductById için bir action adı belirleyip onu kullanacağız.
            return CreatedAtAction(nameof(GetProductById), new { id = productId }, new { id = productId });
        }

        // GET api/products
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListProducts()
        {
            var query = new ListProductsQuery();
            var products = await _mediator.Send(query);
            return Ok(products);
        }

        // GET api/products/{id}
        // Bu endpoint, CreateProduct'taki CreatedAtAction tarafından referans alınacak.
        [HttpGet("{id:guid}", Name = "GetProductById")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var query = new GetProductByIdQuery(id);
            var productDto = await _mediator.Send(query);

            if (productDto == null)
            {
                return NotFound(); // 404 Not Found
            }

            return Ok(productDto); // 200 OK
        }

        // İleride eklenecek diğer endpoint'ler (Update, Delete vb.) buraya gelecek.
    }
}