using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;

namespace PortManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "PortAuthorityOfficer,Administrator")]
public class ShippingAgentOrganizationController : ControllerBase
{
    private readonly IShippingAgentOrganizationService _service;

    public ShippingAgentOrganizationController(IShippingAgentOrganizationService service)
    {
        _service = service;
    }

    // GET: api/shippingagentorganization
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

    // GET: api/shippingagentorganization/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, ct);
            if (result == null) return NotFound(new { error = "Organization not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    // POST: api/shippingagentorganization
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShippingAgentOrganizationDto dto, CancellationToken ct = default)
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

    // PATCH: api/shippingagentorganization/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ShippingAgentOrganizationDto dto, CancellationToken ct = default)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto, ct);
            if (result == null) return NotFound(new { error = "Organization not found" });
            return Ok(result);
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

    // DELETE: api/shippingagentorganization/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id, ct);
            if (!deleted) return NotFound(new { error = "Organization not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }
}
