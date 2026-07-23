using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.Services;
using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Docks.Entities;

namespace PortManagement.Api.Controllers;

// This controller handles HTTP requests related to docks
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "PortAuthorityOfficer,Administrator")]
public class DockController : ControllerBase
{
    private readonly IDockService _service;
    private readonly DockMapper _mapper;


    // Constructor injects the dock service
    public DockController(IDockService service, DockMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    // Returns a list of all docks
    // GET: api/dock
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        try
        {
            var result = await _service.GetAllAsync(ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // Returns a dock by its ID
    // GET: api/dock/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, ct);
            if (result == null) return NotFound(new { error = "Dock not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // Creates a new dock
    // POST: api/dock
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DockDto dto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Validation failed", details = ModelState });
            var entity = await _mapper.ToEntityAsync(dto, ct);
            await _service.AddAsync(entity, ct);
            return Ok(new { message = "Dock added successfully" });

        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new { error = "Validation failed", message = ex.Message });
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = "Business rule violation", message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // Updates an existing dock
    // PATCH: api/dock/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update([FromBody] DockDto dto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Validation failed", details = ModelState });
            var entity = await _mapper.ToEntityAsync(dto, ct);
            await _service.UpdateAsync(entity, ct);
            return Ok(new { message = "Dock updated successfully" });
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new { error = "Validation failed", message = ex.Message });
        }
        catch (BusinessRuleViolationException ex)
        {
            return BadRequest(new { error = "Business rule violation", message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // Deletes a dock
    // DELETE: api/dock/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // Searches for docks based on criteria
    // GET: api/dock/search?name=...&location=...&vesselTypeId=...
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] string? location, [FromQuery] Guid? vesselTypeId, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.SearchAsync(name, location, vesselTypeId, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
