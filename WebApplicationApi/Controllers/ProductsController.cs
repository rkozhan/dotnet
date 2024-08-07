using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationApi.Models;

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;

            // Ensure the database is created. This is typically used for development and testing.
            _context.Database.EnsureCreated();
        }

        // Asynchronous method to get all products
        [HttpGet]
        public async Task<ActionResult>  getAllProducts()
        {
            return Ok(await _context.Products.ToArrayAsync());
        }

        // Asynchronous method to get all available products
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Product>>> getAllAvailableProducts()
        {
            {
                var availableProducts = await _context.Products
                    .Where(p => p.IsAvailable)
                    .ToArrayAsync();

                return Ok(availableProducts);
            }
        }

        // Asynchronous method to get a product by ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product
                );
        }
    }
}
