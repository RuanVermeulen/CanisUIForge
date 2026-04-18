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
public class CustomersController : ControllerBase
{
    private readonly TestApiDbContext _context;

    public CustomersController(TestApiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerResponse>), 200)]
    public async Task<IActionResult> GetAll()
    {
        List<Data.Entities.Customer> customers = await _context.Customers.ToListAsync();
        List<CustomerResponse> response = customers.Select(c => new CustomerResponse
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            CreatedDate = c.CreatedDate
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        Data.Entities.Customer? customer = await _context.Customers.FindAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        CustomerResponse response = new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            CreatedDate = customer.CreatedDate
        };

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        Data.Entities.Customer customer = new Data.Entities.Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            CreatedDate = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        CustomerResponse response = new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            CreatedDate = customer.CreatedDate
        };

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomerResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request)
    {
        Data.Entities.Customer? customer = await _context.Customers.FindAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;

        await _context.SaveChangesAsync();

        CustomerResponse response = new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            CreatedDate = customer.CreatedDate
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        Data.Entities.Customer? customer = await _context.Customers.FindAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(SearchCustomersResponse), 200)]
    public async Task<IActionResult> Search([FromBody] SearchCustomersRequest request)
    {
        IQueryable<Data.Entities.Customer> query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string term = request.SearchTerm.ToLower();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(term) ||
                c.LastName.ToLower().Contains(term) ||
                c.Email.ToLower().Contains(term));
        }

        int totalCount = await query.CountAsync();

        List<Data.Entities.Customer> customers = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        SearchCustomersResponse response = new SearchCustomersResponse
        {
            Items = customers.Select(c => new CustomerResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                CreatedDate = c.CreatedDate
            }).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Ok(response);
    }
}
