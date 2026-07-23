using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.Services;
using PortManagement.Application.DTOs;
using PortManagement.Application.Mappers;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Staff.Entities;
using Microsoft.AspNetCore.Authorization;


namespace PortManagement.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "LogisticsOperator,Administrator")]
public class StaffMemberController : ControllerBase
{
    private readonly IStaffMemberService _service;


    public StaffMemberController(IStaffMemberService service)
    {
        _service = service;
    }


    // GET: api/staffmember
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StaffMemberDTO>>> GetAll()
    {
        var result = await _service.GetAllStaffMembersAsync();
        var dtos = result.Select(StaffMemberMapper.ToDto).ToList();
        return Ok(dtos);
    }


    // GET: api/staffmember/available
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<StaffMemberDTO>>> GetAvailable()
    {
        var result = await _service.GetAvailableStaffMembersAsync();
        var dtos = result.Select(StaffMemberMapper.ToDto).ToList();
        return Ok(dtos);
    }

    // GET: api/staffmember/unavailable
    [HttpGet("unavailable")]
    public async Task<ActionResult<IEnumerable<StaffMemberDTO>>> GetNotAvailable()
    {
        var result = await _service.GetUnavailableStaffMembersAsync();
        var dtos = result.Select(StaffMemberMapper.ToDto).ToList();

        return Ok(dtos);
    }

    // GET: api/staffmember/guid/{id}
    [HttpGet("guid/{id:guid}")]
    public async Task<ActionResult<StaffMemberDTO>> GetByGuId(Guid id)
    {
        var entity = await _service.GetStaffMemberByGuidAsync(id);
        if (entity == null)
            return NotFound();

        var dto = StaffMemberMapper.ToDto(entity);
        return Ok(dto);
    }


    // GET: api/staffmember/id/{staffMemberId}
    [HttpGet("id/{staffMemberId}")]
    public async Task<ActionResult<StaffMemberDTO>> GetById(string id)
    {
        var result = await _service.GetStaffMemberByIdAsync(id);
        if (result == null)
            return NotFound();

        var dto = StaffMemberMapper.ToDto(result);
        return Ok(dto);
    }

    // POST: api/staffmember
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StaffMemberDTO dto)
    {
        if (dto == null)
            return BadRequest("Staff member data cannot be null.");

        StaffMember entity;
        try
        {
            entity = StaffMemberMapper.ToEntity(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        await _service.CreateStaffMemberAsync(entity);
        var resultDto = StaffMemberMapper.ToDto(entity);
        return CreatedAtAction(nameof(GetById), new { staffMemberId = entity.StaffMemberId.Value }, resultDto);
    }


    // PATCH: api/staffmember/{id}
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] StaffMemberDTO dto)
    {
        if (dto == null)
            return BadRequest("Staff member data cannot be null.");

        dto.Id ??= id;

        StaffMember entity;

        try
        {
            entity = StaffMemberMapper.ToEntity(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            await _service.UpdateStaffMemberAsync(entity);
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


    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            await _service.DeactivateStaffMemberAsync(id);
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

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
            await _service.ActivateStaffMemberAsync(id);
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


    // GET: api/staffmember/search/name?name=term
    [HttpGet("search/name")]
    public async Task<ActionResult<StaffMemberDTO>> SearchByName(string name)
    {
        var entities = await _service.GetStaffMembersByNameAsync(name);
        var dtos = entities.Select(StaffMemberMapper.ToDto).ToList();
        return Ok(dtos);
    }


    // GET: api/staffmember/search/qualification?qualification=term
    [HttpGet("search/qualification")]
    public async Task<ActionResult<StaffMemberDTO>> SearchByQualification(string qualification)
    {
        var entities = await _service.GetStaffMembersByQualificationAsync(qualification);
        var dtos = entities.Select(StaffMemberMapper.ToDto).ToList();
        return Ok(dtos);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            await _service.RemoveStaffMemberAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}
