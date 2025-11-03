using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class FormsController : ControllerBase
{
    private readonly ILogger<FormsController> _logger;

    public FormsController(ILogger<FormsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all form templates
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetForms([FromQuery] string companyId)
    {
        _logger.LogInformation("Getting form templates for company: {CompanyId}", companyId);
        return Ok(new List<object>());
    }

    /// <summary>
    /// Create form template
    /// </summary>
    [HttpPost]
    [ProducesResponseType(200)]
    public async Task<IActionResult> CreateForm([FromBody] object request)
    {
        _logger.LogInformation("Creating form template");
        return Ok(new { Status = "success", Response = "Form created" });
    }

    /// <summary>
    /// Update form template
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> UpdateForm(int id, [FromBody] object request)
    {
        _logger.LogInformation("Updating form template: {Id}", id);
        return Ok(new { Status = "success", Response = "Form updated" });
    }

    /// <summary>
    /// Delete form template
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DeleteForm(int id, [FromQuery] string companyId)
    {
        _logger.LogInformation("Deleting form template: {Id}", id);
        return Ok(new { Status = "success", Response = "Form deleted" });
    }
}

