using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(ILogger<AppointmentsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get appointments list
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] string appointmentDate, 
        [FromQuery] string companyId, 
        [FromQuery] string userId, 
        [FromQuery] int appointmentTypeStatus)
    {
        _logger.LogInformation("Getting appointments for date: {Date}, company: {CompanyId}", appointmentDate, companyId);
        // TODO: Implement with repository pattern
        return Ok(new List<object>());
    }

    /// <summary>
    /// Get appointments list with forms
    /// </summary>
    [HttpGet("with-forms")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAppointmentsWithForms(
        [FromQuery] string appointmentDate, 
        [FromQuery] string companyId, 
        [FromQuery] string userId, 
        [FromQuery] int appointmentTypeStatus)
    {
        _logger.LogInformation("Getting appointments with forms for date: {Date}, company: {CompanyId}", appointmentDate, companyId);
        return Ok(new List<object>());
    }

    /// <summary>
    /// Get single appointment
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAppointment(int id)
    {
        _logger.LogInformation("Getting appointment: {Id}", id);
        return Ok(new { id });
    }

    /// <summary>
    /// Create new appointment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        _logger.LogInformation("Creating appointment for customer: {CustomerId}", request.CustomerID);
        return CreatedAtAction(nameof(GetAppointment), new { id = "new-id" }, request);
    }

    /// <summary>
    /// Update existing appointment
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentRequest request)
    {
        _logger.LogInformation("Updating appointment: {Id}", id);
        return Ok(request);
    }

    /// <summary>
    /// Save CSL image
    /// </summary>
    [HttpPost("csl-image")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> SaveCSLImage([FromBody] object request)
    {
        _logger.LogInformation("Saving CSL image");
        return Ok(new { Status = "success", Response = "Image saved" });
    }

    /// <summary>
    /// Get CSL images
    /// </summary>
    [HttpGet("csl-images")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetCSLImages(
        [FromQuery] int appointmentId, 
        [FromQuery] int customerId, 
        [FromQuery] int cslId, 
        [FromQuery] string companyId)
    {
        _logger.LogInformation("Getting CSL images for appointment: {ApptId}", appointmentId);
        return Ok(new List<object>());
    }

    /// <summary>
    /// Assign form to appointment
    /// </summary>
    [HttpPost("assign-form")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> AssignForm([FromBody] object request)
    {
        _logger.LogInformation("Assigning form to appointment");
        return Ok(new { Status = "success", Response = "Form assigned" });
    }
}

public record CreateAppointmentRequest(int CustomerID, string ServiceTypeId, DateTime ApptDateTime);
public record UpdateAppointmentRequest(string Note, string StatusId);

