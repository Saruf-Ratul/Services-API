using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class StatusController : ControllerBase
{
    private readonly ILogger<StatusController> _logger;

    public StatusController(ILogger<StatusController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all appointment statuses
    /// </summary>
    [HttpGet("appointments")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAppointmentStatuses([FromQuery] string companyId)
    {
        _logger.LogInformation("Getting appointment statuses for company: {CompanyId}", companyId);
        return Ok(new List<object>());
    }

    /// <summary>
    /// Get all ticket statuses
    /// </summary>
    [HttpGet("tickets")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetTicketStatuses([FromQuery] string companyId)
    {
        _logger.LogInformation("Getting ticket statuses for company: {CompanyId}", companyId);
        return Ok(new List<object>());
    }
}

