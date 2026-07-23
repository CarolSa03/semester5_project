using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PortManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "PortAuthorityOfficer,Administrator")]
public class VesselRecordController : ControllerBase
{
    private readonly IVesselRecordService _service;
    private readonly IVesselTypeRepository _vesselTypeRepo;

    public VesselRecordController(IVesselRecordService service, IVesselTypeRepository vesselTypeRepo)
    {
        _service = service;
        _vesselTypeRepo = vesselTypeRepo;
    }

    // GET: api/vesselrecord
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VesselRecordDto>>> GetAll(CancellationToken ct = default)
    {
        var entities = await _service.GetAllAsync(ct);
        var dtos = entities.Select(VesselRecordMapper.ToDto).ToList();
        return Ok(dtos);
    }

    // GET: api/vesselrecord/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VesselRecordDto>> GetById(Guid id, CancellationToken ct = default)
    {
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity == null)
            return NotFound();

        var dto = VesselRecordMapper.ToDto(entity);
        return Ok(dto);
    }

    // GET: api/vesselrecord/imo/{imo}
    [HttpGet("imo/{imo}")]
    public async Task<ActionResult<VesselRecordDto>> GetByIMO(int imo, CancellationToken ct = default)
    {
        if (!Imo.TryCreate(imo, out var imoVo))
            return BadRequest(new { error = "Invalid IMO format" });

        var entity = await _service.GetByIMOAsync(imoVo, ct);
        if (entity == null)
            return NotFound();

        var dto = VesselRecordMapper.ToDto(entity);
        return Ok(dto);
    }

    // POST: api/vesselrecord
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] VesselRecordDto dto, CancellationToken ct = default)
    {
        if (dto == null)
            return BadRequest("Body cannot be null.");

        try
        {
            // Parse IMO
            if (!Imo.TryCreate(dto.ImoValue, out var imo))
                return BadRequest("Invalid IMO format");

            // Validate vessel type
            var vesselType = await _vesselTypeRepo.GetByIdAsync(dto.VesselTypeId, ct);
            if (vesselType == null || !vesselType.IsActive)
                return BadRequest("Vessel type not found or inactive");

            // Create value objects
            var vesselName = new VRName(dto.Name);
            var vesselOwner = new VROwner(dto.Owner);
            var status = dto.IsActive ? VRStatus.Active : VRStatus.Inactive;

            // Create entity using constructor
            var entity = new VesselRecord(imo, vesselName, vesselType, vesselOwner, status);

            await _service.AddAsync(entity, ct);

            var resultDto = VesselRecordMapper.ToDto(entity);
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

    // PUT: api/vesselrecord/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] VesselRecordDto dto, CancellationToken ct = default)
    {
        if (dto == null)
            return BadRequest("Body cannot be null.");

        dto.Id ??= id;

        try
        {
            var entity = VesselRecordMapper.ToEntity(dto);
            //entity.SetId(id);

            await _service.UpdateAsync(entity, ct);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // PATCH: api/vesselrecord/{id}/inactivate
    [HttpPatch("{id:guid}/inactivate")]
    public async Task<IActionResult> Inactivate(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _service.InactivateAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE: api/vesselrecord/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // GET: api/vesselrecord/search?q=term
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<VesselRecordDto>>> Search([FromQuery] string q, CancellationToken ct = default)
    {
        var entities = await _service.SearchAsync(q, ct);
        var dtos = entities.Select(VesselRecordMapper.ToDto).ToList();
        return Ok(dtos);
    }
}
