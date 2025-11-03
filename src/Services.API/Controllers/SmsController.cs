using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class SmsController : ControllerBase
{
    private readonly ILogger<SmsController> _logger;

    public SmsController(ILogger<SmsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Send SMS to customer
    /// </summary>
    [HttpPost("send")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> SendSms([FromBody] object request)
    {
        _logger.LogInformation("Sending SMS");
        return Ok(new { Status = "success", Response = "SMS sent" });
    }
}

