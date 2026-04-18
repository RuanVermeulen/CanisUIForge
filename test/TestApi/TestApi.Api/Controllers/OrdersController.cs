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
public class OrdersController : ControllerBase
{
    private readonly TestApiDbContext _context;

    public OrdersController(TestApiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<OrderResponse>), 200)]
    public async Task<IActionResult> GetAll()
    {
        List<Data.Entities.Order> orders = await _context.Orders.ToListAsync();
        List<OrderResponse> response = orders.Select(o => new OrderResponse
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            ProductId = o.ProductId,
            Quantity = o.Quantity,
            Status = o.Status,
            OrderDate = o.OrderDate
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        Data.Entities.Order? order = await _context.Orders.FindAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        OrderResponse response = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            Status = order.Status,
            OrderDate = order.OrderDate
        };

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        Data.Entities.Order order = new Data.Entities.Order
        {
            CustomerId = request.CustomerId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Status = "Pending",
            OrderDate = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        OrderResponse response = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            Status = order.Status,
            OrderDate = order.OrderDate
        };

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrderResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderRequest request)
    {
        Data.Entities.Order? order = await _context.Orders.FindAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        order.CustomerId = request.CustomerId;
        order.ProductId = request.ProductId;
        order.Quantity = request.Quantity;
        order.Status = request.Status;

        await _context.SaveChangesAsync();

        OrderResponse response = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            Status = order.Status,
            OrderDate = order.OrderDate
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        Data.Entities.Order? order = await _context.Orders.FindAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
