using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class PaymentLinkController : ControllerBase
{
    private readonly ILogger<PaymentLinkController> _logger;

    public PaymentLinkController(ILogger<PaymentLinkController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get XPay link
    /// </summary>
    [HttpGet("xpay")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetXPayLink(
        [FromQuery] string rmCompanyID,
        [FromQuery] string customerID,
        [FromQuery] string invoiceNo,
        [FromQuery] string customerName,
        [FromQuery] string email,
        [FromQuery] string amount)
    {
        _logger.LogInformation("Getting XPay link for invoice: {InvoiceNo}", invoiceNo);
        return Ok(new { XPayLink = "https://payment-link-placeholder.com" });
    }

    /// <summary>
    /// Generate XPay link
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GenerateXPayLink([FromBody] object request)
    {
        _logger.LogInformation("Generating XPay link");
        return Ok(new { XPayLink = "https://payment-link-placeholder.com" });
    }
}

