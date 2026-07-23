using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.PhysicalResources.Entities;

namespace PortManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "LogisticsOperator,Administrator")]
public class PhysicalResourceController : ControllerBase
{
    private readonly IPhysicalResourceService _service;

    public PhysicalResourceController(IPhysicalResourceService service)
    {
        _service = service;
    }

    // Get: api/PhysicalResource
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhysicalResourceDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        var dtos = entities.Select(PhysicalResourceMapper.ToDto).ToList();
        return Ok(dtos);
    }

    // Get: api/PhysicalResource/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PhysicalResourceDto>> GetById(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var dto = PhysicalResourceMapper.ToDto(entity);
        return Ok(dto);
    }

    // Get: api/PhysicalResource/trucks
    [HttpGet("trucks")]
    public async Task<ActionResult<IEnumerable<TruckDto>>> GetTrucks()
    {
        var entities = await _service.GetByTypeAsync<Truck>();
        var dtos = entities.Select(e => (TruckDto)e).ToList();
        return Ok(dtos);
    }

    // Get: api/PhysicalResource/yardcranes
    [HttpGet("yardcranes")]
    public async Task<ActionResult<IEnumerable<YardCraneDto>>> GetYardCranes()
    {
        var entities = await _service.GetByTypeAsync<YardCrane>();
        var dtos = entities.Select(e => (YardCraneDto)e).ToList();
        return Ok(dtos);
    }

    // Get: api/PhysicalResource/stscranes
    [HttpGet("stscranes")]
    public async Task<ActionResult<IEnumerable<STSCraneDto>>> GetSTSCranes()
    {
        var entities = await _service.GetByTypeAsync<STSCrane>();
        var dtos = entities.Select(e => (STSCraneDto)e).ToList();
        return Ok(dtos);
    }

    // Post: api/PhysicalResource
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] PhysicalResourceDto dto)
    {
        if (dto is null) return BadRequest("Body cannot be null.");

        PhysicalResource entity;
        try
        {
            entity = PhysicalResourceMapper.ToEntity(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        await _service.AddAsync(entity);

        var resultDto = PhysicalResourceMapper.ToDto(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, resultDto);
    }


    // Post: api/PhysicalResource/truck
    [HttpPost("truck")]
    public async Task<ActionResult> CreateTruck([FromBody] TruckDto dto)
    {
        if (dto is null) return BadRequest("Body cannot be null.");

        try
        {
            var entity = PhysicalResourceMapper.ToEntity(dto);
            await _service.AddAsync(entity);

            var resultDto = PhysicalResourceMapper.ToDto(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, resultDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // Post: api/PhysicalResource/yardcrane
    [HttpPost("yardcrane")]
    public async Task<ActionResult> CreateYardCrane([FromBody] YardCraneDto dto)
    {
        if (dto is null) return BadRequest("Body cannot be null.");

        try
        {
            var entity = PhysicalResourceMapper.ToEntity(dto);
            await _service.AddAsync(entity);

            var resultDto = PhysicalResourceMapper.ToDto(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, resultDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // Post: api/PhysicalResource/stscrane
    [HttpPost("stscrane")]
    public async Task<ActionResult> CreateSTSCrane([FromBody] STSCraneDto dto)
    {
        if (dto is null) return BadRequest("Body cannot be null.");

        try
        {
            var entity = PhysicalResourceMapper.ToEntity(dto);
            await _service.AddAsync(entity);

            var resultDto = PhysicalResourceMapper.ToDto(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, resultDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }


    // Put: api/PhysicalResource/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] PhysicalResourceDto dto)
    {

        if (dto == null)
            return BadRequest("Body cannot be null.");

        dto.Id ??= id;

        PhysicalResource entity;
        try
        {
            entity = PhysicalResourceMapper.ToEntity(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            await _service.UpdateAsync(entity);
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

    // Patch: api/PhysicalResource/{id}/deactivate
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
