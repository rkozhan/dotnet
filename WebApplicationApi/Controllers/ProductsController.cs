using HPlusSport.API.Models;
using Microsoft.AspNetCore.Http;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationApi.Models;

namespace WebApplicationApi.Controllers
{
    [ApiVersion("2.0")]
    //[Route("api/v{version:apiVersion}/products")]
    [Route("api/products")]
    [ApiController]
    public class ProductsV2Controller : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsV2Controller(ShopContext context)
        {
            _context = context;

            // Ensure the database is created. This is typically used for development and testing.
            _context.Database.EnsureCreated();
        }

        // Asynchronous method to get all products
        [HttpGet]
        public async Task<ActionResult>  getAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products.Where(p => p.IsAvailable == true);

            if(queryParameters.MinPrice != null)
            {
                products = products
                    .Where(p => p.Price >= queryParameters.MinPrice.Value);
            }

            if (queryParameters.MaxPrice != null)
            {
                products = products
                    .Where(p => p.Price <= queryParameters.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                products = products
                    .Where(p => p.Sku.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
                                p.Name.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products
                    .Where(p => p.Sku == queryParameters.Sku);
            }

            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products
                    .Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null) 
                    {
                    products = products.OrderByCustom( queryParameters.SortBy, queryParameters.SortOrder);
                    }
            }

            products = products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
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

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            
            _context.Entry(product).State = EntityState.Modified;

            try
            {
            await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exceptions that occur when the record being updated no longer exists in the database
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                } else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct( int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery] int[] ids)
        {
            var products = new List<Product>();

            foreach(var id in ids)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                } else
                {
                    products.Add(product);
                }
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();
            return Ok(products);
        }
    }

    [ApiVersion("1.0")]
    //[Route("api/v{version:apiVersion}/products")]
    [Route("api/products")]
    [ApiController]
    public class ProductsV1Controller : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsV1Controller(ShopContext context)
        {
            _context = context;

            // Ensure the database is created. This is typically used for development and testing.
            _context.Database.EnsureCreated();
        }

        // Asynchronous method to get all products
        [HttpGet]
        public async Task<ActionResult> getAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;

            if (queryParameters.MinPrice != null)
            {
                products = products
                    .Where(p => p.Price >= queryParameters.MinPrice.Value);
            }

            if (queryParameters.MaxPrice != null)
            {
                products = products
                    .Where(p => p.Price <= queryParameters.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                products = products
                    .Where(p => p.Sku.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
                                p.Name.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products
                    .Where(p => p.Sku == queryParameters.Sku);
            }

            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products
                    .Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            products = products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
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

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exceptions that occur when the record being updated no longer exists in the database
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery] int[] ids)
        {
            var products = new List<Product>();

            foreach (var id in ids)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }
                else
                {
                    products.Add(product);
                }
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();
            return Ok(products);
        }
    }
}
