using Microsoft.AspNetCore.Mvc;
using PaymentsService.Models;
using PaymentsService.Services;

namespace PaymentsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-account")]
    public async Task<ActionResult<Account>> CreateAccount([FromBody] CreateAccountRequest request, [FromHeader] string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required");

        try
        {
            var account = await _paymentService.CreateAccountAsync(userId, request.InitialBalance);
            return CreatedAtAction(nameof(GetAccount), new { userId = account.UserId }, account);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("add-funds")]
    public async Task<ActionResult> AddFunds([FromBody] AddFundsRequest request, [FromHeader] string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required");

        try
        {
            // ИСПРАВЛЕНО: Используем правильный метод DepositAsync
            await _paymentService.DepositAsync(userId, request.Amount);
            return Ok(new { Message = "Funds added successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-account")]
    public async Task<ActionResult<Account>> GetAccount([FromHeader] string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required");

        try
        {
            // ИСПРАВЛЕНО: Создаем метод для получения полной информации об аккаунте
            var balance = await _paymentService.GetBalanceAsync(userId);
            if (balance == null)
                return NotFound("Account not found");

            // Возвращаем объект с балансом
            return Ok(new { UserId = userId, Balance = balance.Value });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{userId}/balance")]
    public async Task<ActionResult<decimal>> GetBalance(string userId)
    {
        var balance = await _paymentService.GetBalanceAsync(userId);
        if (balance == null)
            return NotFound("Account not found");

        return Ok(new { Balance = balance.Value });
    }
}

public class CreateAccountRequest
{
    public decimal InitialBalance { get; set; }
}

public class AddFundsRequest
{
    public decimal Amount { get; set; }
}
