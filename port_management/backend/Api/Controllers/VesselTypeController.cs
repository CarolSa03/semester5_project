using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.Services;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;

namespace PortManagement.Api.Controllers;

// This controller handles HTTP requests related to vessel types
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "PortAuthorityOfficer,Administrator")]
public class VesselTypeController : ControllerBase
{
    private readonly IVesselTypeService _service;

    // Constructor injects the vessel type service
    public VesselTypeController(IVesselTypeService service)
    {
        _service = service;
    }

    // Returns all vessel types (anonymous access allowed)
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    // Returns all active vessel types (anonymous access allowed)
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken ct = default)
    {
        var result = await _service.GetActiveAsync(ct);
        return Ok(result);
    }

    // Returns a vessel type by its ID (anonymous access allowed)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (result == null) return NotFound(new { error = "Vessel type not found" });
        return Ok(result);
    }

    // Creates a new vessel type
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VesselTypeDto dto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Validation failed", details = ModelState });

            var result = await _service.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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

    // Updates an existing vessel type
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] VesselTypeDto dto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Validation failed", details = ModelState });

            var result = await _service.UpdateAsync(id, dto, ct);
            if (result == null) return NotFound(new { error = "Vessel type not found" });
            return Ok(result);
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

    // Inactivates a vessel type
    [HttpPatch("{id}/inactivate")]
    public async Task<IActionResult> Inactivate(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.InactivateAsync(id, ct);
            if (result == null) return NotFound(new { error = "Vessel type not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    // Deletes a vessel type
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.DeleteAsync(id, ct);
            if (result == null) return NotFound(new { error = "Vessel type not found" });
            return Ok(result);
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

    // Searches for vessel types by a query string
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.SearchAsync(q, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
