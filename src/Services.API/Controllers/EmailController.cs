using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;

    public EmailController(ILogger<EmailController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Send HTML formatted email
    /// </summary>
    [HttpPost("send")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> SendEmail([FromBody] object request)
    {
        _logger.LogInformation("Sending email");
        return Ok(new { IsValid = true, Message = "Sent" });
    }

    /// <summary>
    /// Get auto-fill values for email
    /// </summary>
    [HttpGet("autofill")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAutoFillValues([FromQuery] string companyId)
    {
        _logger.LogInformation("Getting auto-fill values for company: {CompanyId}", companyId);
        return Ok(new { });
    }
}

