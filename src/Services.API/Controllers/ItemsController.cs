using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(ILogger<ItemsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all items
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetItems([FromQuery] string companyId)
    {
        _logger.LogInformation("Getting items for company: {CompanyId}", companyId);
        return Ok(new List<object>());
    }
}

