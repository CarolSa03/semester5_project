using PortManagement.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Qualification.Entities;
using Microsoft.AspNetCore.Authorization;


namespace PortManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "LogisticsOperator,Administrator")]
public class QualificationController : ControllerBase
{
    private readonly IQualificationService _service;

    public QualificationController(IQualificationService service)
    {
        _service = service;
    }

    // GET: api/qualification
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QualificationDto>>> GetAll()
    {
        var result = await _service.GetAllQualificationsAsync();
        var dtos = result.Select(QualificationMapper.ToDto).ToList();
        return Ok(dtos);
    }

    // GET: api/qualification/{code}
    [HttpGet("{code}")]
    public async Task<ActionResult<QualificationDto>> GetById(string code)
    {
        var result = await _service.GetQualificationByCodeAsync(code);
        if (result == null)
            return NotFound();

        var dto = QualificationMapper.ToDto(result);
        return Ok(dto);
    }

    // GET: api/qualification/name/{descriptiveName}
    [HttpGet("name/{descriptiveName}")]
    public async Task<ActionResult<QualificationDto>> GetByDescriptiveName(string descriptiveName)
    {
        var result = await _service.GetQualificationByNameAsync(descriptiveName);
        if (result == null)
            return NotFound();

        var dto = QualificationMapper.ToDto(result);
        return Ok(dto);
    }

    // POST: api/qualification
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QualificationDto dto)
    {
        if (dto == null)
            return BadRequest("Qualification data is required.");

        Qualification entity;

        try
        {
            entity = QualificationMapper.ToEntity(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        await _service.CreateQualificationAsync(entity);

        var result = QualificationMapper.ToDto(entity);

        return CreatedAtAction(nameof(GetById), new { code = entity.Code.Value }, result);
    }

    // PATCH: api/qualification/{id}
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] QualificationDto dto)
    {
        if (dto == null)
            return BadRequest("Qualification data is required.");

        dto.Id ??= id;

        Qualification entity;
        try
        {
            entity = QualificationMapper.ToEntity(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            await _service.UpdateQualificationAsync(entity);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetQualificationByIdAsync(id);
        if (result == null)
            return NotFound();
        var dto = QualificationMapper.ToDto(result);
        return Ok(dto);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        var qualification = await _service.GetQualificationByCodeAsync(code);
        if (qualification == null)
            return NotFound($"Qualification with code '{code}' not found.");

        await _service.DeleteQualificationAsync(code);
        return NoContent();
    }
}
