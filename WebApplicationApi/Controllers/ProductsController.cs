using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IEnumerable<Product> getAllProducts()
        {
            return _context.Products.ToArray();
        }
    }
}
