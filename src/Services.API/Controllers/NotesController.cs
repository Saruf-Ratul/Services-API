using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion(1.0)]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly ILogger<NotesController> _logger;

    public NotesController(ILogger<NotesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all notes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetNotes([FromQuery] string companyId)
    {
        _logger.LogInformation("Getting notes for company: {CompanyId}", companyId);
        return Ok(new List<object>());
    }

    /// <summary>
    /// Create note
    /// </summary>
    [HttpPost]
    [ProducesResponseType(200)]
    public async Task<IActionResult> CreateNote([FromBody] object request)
    {
        _logger.LogInformation("Creating note");
        return Ok(new { Status = "success", Response = "Note created" });
    }

    /// <summary>
    /// Update note
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] object request)
    {
        _logger.LogInformation("Updating note: {Id}", id);
        return Ok(new { Status = "success", Response = "Note updated" });
    }

    /// <summary>
    /// Delete note
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DeleteNote(int id, [FromQuery] string companyId)
    {
        _logger.LogInformation("Deleting note: {Id}", id);
        return Ok(new { Status = "success", Response = "Note deleted" });
    }
}

