using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ILogger<CustomersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] string? date, [FromQuery] string? companyId)
    {
        _logger.LogInformation("Getting customers for date: {Date}, company: {CompanyId}", date, companyId);
        // TODO: Implement with repository pattern
        return Ok(new List<object>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(string id)
    {
        _logger.LogInformation("Getting customer: {Id}", id);
        return Ok(new { id });
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        _logger.LogInformation("Creating customer: {FirstName} {LastName}", request.FirstName, request.LastName);
        return CreatedAtAction(nameof(GetCustomer), new { id = "new-id" }, request);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "UserOrAdmin")]
    public async Task<IActionResult> UpdateCustomer(string id, [FromBody] UpdateCustomerRequest request)
    {
        _logger.LogInformation("Updating customer: {Id}", id);
        return Ok(request);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        _logger.LogInformation("Deleting customer: {Id}", id);
        return NoContent();
    }
}

public record CreateCustomerRequest(string FirstName, string LastName, string Email, string CompanyID);
public record UpdateCustomerRequest(string FirstName, string LastName, string Email);

