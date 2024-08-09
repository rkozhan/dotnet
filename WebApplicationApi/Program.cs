using HPlusSport.API.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-API-Version"),
        new MediaTypeApiVersionReader("ver"));
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopContext>(options =>
{
    options.UseInMemoryDatabase("Shop");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapGet("/products", async (ShopContext _context, [AsParameters] ProductQueryParameters queryParameters) =>
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

    return await _context.Products.ToArrayAsync();
});

app.MapGet("/products/available", async (ShopContext _context) =>
    Results.Ok(await _context.Products.Where(p => p.IsAvailable).ToArrayAsync())
);

app.MapGet("products/{id}", async (int id, ShopContext _context) =>
{
    var product = await _context.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(product);
}).WithName("GetProduct");

app.MapPost("/products/", async (ShopContext _context, Product product) => 
{
    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    return Results.CreatedAtRoute(
        "GetProduct",
        new { id = product.Id },
        product);
});

app.MapPut("/products/{id}", async (ShopContext _context, int id, Product product) =>
{
    if (id != product.Id)
    {
        return Results.BadRequest();
    }

    _context.Entry(product).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.Products.Any(p => p.Id == id))
        {
            return Results.NotFound();
        } else
        {
            throw;
        }
    }

    return Results.NoContent();
});

app.MapDelete("/products/{id}", async (ShopContext _context, int id) =>
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
    {
        return Results.NotFound();
    }

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();

    return Results.Ok(product);
});

app.MapDelete("/products/delete", async (ShopContext _context, [FromQuery] int[] ids) =>
{
    var products = new List<Product>();

    foreach (var id in ids)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return Results.NotFound();
        }
        else
        {
            products.Add(product);
        }
    }

    _context.Products.RemoveRange(products);
    await _context.SaveChangesAsync();
    return Results.Ok(products);
});


app.Run();
