using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.DTOs.Auth;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Auth;

namespace PortManagement.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
//    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuditService _auditService;

        public AdminController(IAuthService authService, IAuditService auditService)
        {
            _authService = authService;
            _auditService = auditService;
        }

        [HttpPost("users")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AppUserDto>> CreateUser([FromBody] CreateAppUserDto dto)
        {
            try
            {
                var adminUserId = GetCurrentUserIamId();
                var user = await _authService.CreateUserForActivationAsync(dto, adminUserId);

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("users")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<AppUserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserDto>> GetUser(Guid id)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("users/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserDto>> UpdateUser(Guid id, [FromBody] UpdateAppUserDto dto)
        {
            try
            {
                var adminUserId = GetCurrentUserIamId();
                var user = await _authService.UpdateUserProfileAsync(id, dto, adminUserId);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("users/{id}/roles")]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserDto>> AssignRole(Guid id, [FromBody] AssignRoleDto dto)
        {
            try
            {
                var adminUserId = GetCurrentUserIamId();
                var user = await _authService.AssignRoleToUserAsync(id, dto.Role, adminUserId);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("users/{id}/roles/{role}")]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserDto>> RemoveRole(Guid id, Role role)
        {
            try
            {
                var adminUserId = GetCurrentUserIamId();
                var user = await _authService.RemoveRoleFromUserAsync(id, role, adminUserId);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("users/{id}/deactivate")]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserDto>> DeactivateUser(Guid id)
        {
            try
            {
                var adminUserId = GetCurrentUserIamId();
                var user = await _authService.DeactivateUserAsync(id, adminUserId);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("users/{id}/activate")]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserDto>> ActivateUser(Guid id)
        {
            try
            {
                var adminUserId = GetCurrentUserIamId();
                var user = await _authService.ActivateUserAccountAsync(id, adminUserId);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("audit/security")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetSecurityEvents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var events = await _auditService.GetSecurityCriticalEventsAsync(page, pageSize);
            return Ok(events);
        }

        [HttpGet("audit/failures")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetFailureEvents(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var events = await _auditService.GetFailureEventsAsync(page, pageSize);
            return Ok(events);
        }

        [HttpGet("audit/user/{iamUserId}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetUserAuditLogs(
            string iamUserId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var logs = await _auditService.GetAuditLogsByUserAsync(iamUserId, page, pageSize);
            return Ok(logs);
        }

        [HttpGet("audit/date-range")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (startDate > endDate)
                return BadRequest(new { error = "Start date must be before end date" });

            var logs = await _auditService.GetAuditLogsByDateRangeAsync(startDate, endDate, page, pageSize);
            return Ok(logs);
        }

        private string GetCurrentUserIamId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? "Unknown";
        }
    }
}
