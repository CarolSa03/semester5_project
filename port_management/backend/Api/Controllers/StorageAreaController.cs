using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Storage.Entities;

namespace PortManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "PortAuthorityOfficer,Administrator")]
public class StorageAreaController : ControllerBase
{
    private readonly IStorageAreaService _service;

    public StorageAreaController(IStorageAreaService service)
    {
        _service = service;
    }

    // GET: api/storagearea
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StorageAreaDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        var dtos = entities.Select(StorageAreaMapper.ToDto).ToList();
        return Ok(dtos);
    }

    // GET: api/storagearea/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StorageAreaDto>> GetById(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var dto = StorageAreaMapper.ToDto(entity);
        return Ok(dto);
    }

    // GET: api/storagearea/business/{businessId}
    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<StorageAreaDto>> GetByBusinessId(string businessId)
    {
        var entity = await _service.GetByBusinessIdAsync(businessId);
        if (entity == null)
            return NotFound();

        var dto = StorageAreaMapper.ToDto(entity);
        return Ok(dto);
    }

    // POST: api/storagearea
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] StorageAreaDto dto)
    {
        if (dto == null)
            return BadRequest("Body cannot be null.");

        StorageArea entity;
        try
        {
            entity = StorageAreaMapper.ToEntity(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            await _service.AddAsync(entity);

            var resultDto = StorageAreaMapper.ToDto(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, resultDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // PUT: api/storagearea/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] StorageAreaDto dto)
    {
        if (dto == null)
            return BadRequest("Body cannot be null.");

        dto.Id ??= id;

        StorageArea entity;
        try
        {
            entity = StorageAreaMapper.ToEntity(dto);
            //entity.SetId(id);
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
    }

    // DELETE: api/storagearea/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
