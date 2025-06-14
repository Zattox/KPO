using Microsoft.AspNetCore.Mvc;
using OrdersService.Models;
using OrdersService.Services;

namespace OrdersService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request, [FromHeader] string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required");

        try
        {
            var order = await _orderService.CreateOrderAsync(userId, request.Amount, request.Description);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetOrders([FromHeader] string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required");

        try
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }
}

public class CreateOrderRequest
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}