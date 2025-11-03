using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class TaxController : ControllerBase
{
    private readonly ILogger<TaxController> _logger;

    public TaxController(ILogger<TaxController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all taxes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetTaxes([FromQuery] string companyId)
    {
        _logger.LogInformation("Getting taxes for company: {CompanyId}", companyId);
        return Ok(new List<object>());
    }
}

