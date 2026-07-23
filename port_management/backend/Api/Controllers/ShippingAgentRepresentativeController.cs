using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;

namespace PortManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "PortAuthorityOfficer,Administrator")]
public class ShippingAgentRepresentativeController : ControllerBase
{
    private readonly IShippingAgentRepresentativeService _service;

    public ShippingAgentRepresentativeController(IShippingAgentRepresentativeService service)
    {
        _service = service;
    }

    // GET: api/shippingagentrepresentative
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
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    // GET: api/shippingagentrepresentative/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, ct);
            if (result == null) return NotFound(new { error = "Representative not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    // GET: api/shippingagentrepresentative/organization/{organizationId}
    [HttpGet("organization/{organizationId}")]
    public async Task<IActionResult> GetByOrganization(string organizationId, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.GetByOrganizationIdAsync(organizationId, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    // POST: api/shippingagentrepresentative
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShippingAgentRepresentativeDto dto, CancellationToken ct = default)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Validation failed", details = ModelState });

            var result = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "Validation failed", message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = "Conflict", message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    // PATCH: api/shippingagentrepresentative/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ShippingAgentRepresentativeDto dto, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto, ct);
            if (result == null) return NotFound(new { error = "Representative not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    // DELETE: api/shippingagentrepresentative/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id, ct);
            if (!deleted) return NotFound(new { error = "Representative not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }
}
