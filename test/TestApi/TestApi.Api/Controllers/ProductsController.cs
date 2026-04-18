using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApi.Contracts.Requests;
using TestApi.Contracts.Responses;
using TestApi.Data;

namespace TestApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly TestApiDbContext _context;

    public ProductsController(TestApiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProductResponse>), 200)]
    public async Task<IActionResult> GetAll()
    {
        List<Data.Entities.Product> products = await _context.Products.ToListAsync();
        List<ProductResponse> response = products.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            CreatedDate = p.CreatedDate
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        Data.Entities.Product? product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        ProductResponse response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CreatedDate = product.CreatedDate
        };

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        Data.Entities.Product product = new Data.Entities.Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedDate = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        ProductResponse response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CreatedDate = product.CreatedDate
        };

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        Data.Entities.Product? product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;

        await _context.SaveChangesAsync();

        ProductResponse response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CreatedDate = product.CreatedDate
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        Data.Entities.Product? product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
