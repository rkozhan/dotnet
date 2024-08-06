﻿using Microsoft.AspNetCore.Http;
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
    }
}
