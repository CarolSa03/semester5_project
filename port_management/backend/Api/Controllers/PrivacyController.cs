using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.Services.IServices;

namespace PortManagement.Api.Controllers
{
    [ApiController]
    [Route("api/privacy")]
    public class PrivacyController : ControllerBase
    {
        private readonly IPrivacyPolicyService _service;

        public PrivacyController(IPrivacyPolicyService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrent()
        {
            var policy = await _service.GetCurrentAsync();
            if (policy == null)
            {
                return NotFound(new { error = "No active privacy policy found" });
            }
            return Ok(policy);
        }

        [HttpGet("history")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var policies = await _service.GetAllAsync();
            return Ok(policies);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] string content)
        {
            try
            {
                var userId = GetCurrentUserIamId();
                await _service.CreateAsync(content, userId);
                return Ok(new { message = "Privacy policy created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("check")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckNotification()
        {
            var userId = GetCurrentUserIamId();
            var needs = await _service.NeedsNotificationAsync(userId);
            return Ok(new { needsNotification = needs });
        }

        [HttpPost("view")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkViewed()
        {
            try
            {
                var userId = GetCurrentUserIamId();
                await _service.MarkAsViewedAsync(userId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private string GetCurrentUserIamId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? "Unknown";
        }
    }
}
