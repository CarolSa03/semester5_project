using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.Services;
using PortManagement.Application.DTOs;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Vessel.Enums;

namespace PortManagement.Api.Controllers
{
    [ApiController]
    [Route("api/vessel-visit-notifications")]
    [Authorize(Roles = "ShippingAgentRepresentative,PortAuthorityOfficer,Administrator")]
    public class VesselVisitNotificationController : ControllerBase
    {
        private readonly IVesselVisitNotificationService _service;
        private readonly ILogger<VesselVisitNotificationController> _logger;

        public VesselVisitNotificationController(
            IVesselVisitNotificationService service,
            ILogger<VesselVisitNotificationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all vessel visit notifications
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<VesselVisitNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            try
            {
                var notifications = await _service.GetAllAsync(ct);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all notifications");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Creates a new vessel visit notification
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(VesselVisitNotificationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateVesselVisitNotificationDto dto, CancellationToken ct = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Validation failed", details = ModelState });
                }

                var serviceDto = new VesselVisitNotificationDto
                {
                    BusinessId = dto.BusinessId,
                    VesselId = dto.VesselId,
                    ShippingAgentRepresentativeId = dto.ShippingAgentRepresentativeId,
                    ETA = dto.ETA,
                    ETD = dto.ETD
                };

                var result = await _service.CreateAsync(serviceDto, ct);
                return CreatedAtAction(nameof(GetByBusinessId), new { businessId = result.BusinessId }, result);
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for create notification");
                return BadRequest(new { error = "Validation failed", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on create notification");
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Updates an existing vessel visit notification
        /// </summary>
        [HttpPut("{businessId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string businessId, [FromBody] VesselVisitNotificationDto dto, CancellationToken ct = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Validation failed", details = ModelState });
                }

                await _service.UpdateAsync(businessId, dto, ct);
                return NoContent();
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for update notification {BusinessId}", businessId);
                return NotFound(new { error = "Not found", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on update notification {BusinessId}", businessId);
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification {BusinessId}", businessId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Deletes a vessel visit notification
        /// </summary>
        [HttpDelete("{businessId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string businessId, CancellationToken ct = default)
        {
            try
            {
                await _service.DeleteAsync(businessId, ct);
                return NoContent();
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Notification not found for deletion {BusinessId}", businessId);
                return NotFound(new { error = "Not found", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on delete notification {BusinessId}", businessId);
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {BusinessId}", businessId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Submits a vessel visit notification for approval
        /// </summary>
        [HttpPatch("{businessId}/submit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Submit(string businessId, CancellationToken ct = default)
        {
            try
            {
                await _service.SubmitAsync(businessId, ct);
                return NoContent();
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Notification not found for submission {BusinessId}", businessId);
                return NotFound(new { error = "Not found", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on submit notification {BusinessId}", businessId);
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting notification {BusinessId}", businessId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Retrieves a vessel visit notification by business ID
        /// </summary>
        [HttpGet("{businessId}")]
        [ProducesResponseType(typeof(VesselVisitNotificationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByBusinessId(string businessId, CancellationToken ct = default)
        {
            try
            {
                var result = await _service.GetByBusinessIdAsync(businessId, ct);
                if (result == null)
                    return NotFound(new { error = "Notification not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification {BusinessId}", businessId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Withdraws a vessel visit notification
        /// </summary>
        [HttpPatch("{businessId}/withdraw")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Withdraw(
            string businessId,
            [FromBody] WithdrawVesselVisitDto? dto = null,
            CancellationToken ct = default)
        {
            try
            {
                await _service.WithdrawAsync(businessId, dto?.WithdrawalReason, ct);
                return NoContent();
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Notification not found for withdrawal {BusinessId}", businessId);
                return NotFound(new { error = "Not found", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on withdraw notification {BusinessId}", businessId);
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error withdrawing notification {BusinessId}", businessId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Approves a vessel visit notification
        /// </summary>
        [HttpPatch("{businessId}/approve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Approve(
            string businessId,
            [FromBody] ApproveVesselVisitDto dto,
            CancellationToken ct = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Validation failed", details = ModelState });
                }

                // Try to get officer ID from claims, use default if not found
                var officerIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
                Guid officerId;

                if (string.IsNullOrEmpty(officerIdClaim) || !Guid.TryParse(officerIdClaim, out officerId))
                {
                    // Use a default system officer ID for development/testing
                    officerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                    _logger.LogWarning("Officer ID not found in claims, using system default for approval of {BusinessId}", businessId);
                }

                await _service.ApproveAsync(businessId, dto.DockId, officerId, dto.ApprovalNotes, ct);
                return NoContent();
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for approve notification {BusinessId}", businessId);
                return NotFound(new { error = "Not found", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on approve notification {BusinessId}", businessId);
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving notification {BusinessId}", businessId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }


        /// <summary>
        /// Rejects a vessel visit notification
        /// </summary>
        [HttpPatch("{businessId}/reject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Reject(
            string businessId,
            [FromBody] RejectVesselVisitDto dto,
            CancellationToken ct = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Validation failed", details = ModelState });
                }

                var officerIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
                Guid officerId;

                if (string.IsNullOrEmpty(officerIdClaim) || !Guid.TryParse(officerIdClaim, out officerId))
                {
                    officerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                    _logger.LogWarning("Officer ID not found in claims, using system default for rejection of {BusinessId}", businessId);
                }

                await _service.RejectAsync(businessId, dto.RejectionReason, officerId, ct);
                return NoContent();
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for reject notification {BusinessId}", businessId);
                return NotFound(new { error = "Not found", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on reject notification {BusinessId}", businessId);
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting notification {BusinessId}", businessId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Retrieves all pending notifications for the port authority
        /// </summary>
        [HttpGet("pending")]
        [ProducesResponseType(typeof(IEnumerable<VesselVisitNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPendingNotifications(CancellationToken ct = default)
        {
            try
            {
                var result = await _service.GetPendingNotificationsAsync(ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending notifications");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Retrieves vessel visit notifications by shipping agent
        /// </summary>
        [HttpGet("agent/{agentId}")]
        [ProducesResponseType(typeof(IEnumerable<VesselVisitNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByShippingAgent(int agentId, CancellationToken ct = default)
        {
            try
            {
                var result = await _service.GetByShippingAgentAsync(agentId, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for agent {AgentId}", agentId);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Retrieves vessel visit notifications by status
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<VesselVisitNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStatus(string status, CancellationToken ct = default)
        {
            try
            {
                if (!Enum.TryParse<VesselNotificationStatus>(status, true, out var statusEnum))
                {
                    return BadRequest(new { error = "Invalid status", validStatuses = Enum.GetNames<VesselNotificationStatus>() });
                }

                var result = await _service.GetByStatusAsync(statusEnum, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications by status {Status}", status);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Searches for vessel visit notifications based on various criteria
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<VesselVisitNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // csharp
        public async Task<IActionResult> Search(
    [FromQuery] string? periodStart = null,
    [FromQuery] string? periodEnd = null,
    [FromQuery] string? vesselId = null,
    [FromQuery] string? statuses = null,
    [FromQuery] string? businessId = null,
    [FromQuery] int? businessIdYear = null,
    CancellationToken ct = default)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int requestingRepId))
                {
                    return BadRequest(new { error = "Representative ID is required and must be in user claims" });
                }

                // Monta o DTO unificado com os campos que o serviço espera
                var parsedStatuses = ParseStatuses(statuses);
                string? statusString = parsedStatuses != null && parsedStatuses.Any() ? parsedStatuses.First().ToString() : null;

                var dto = new VesselVisitNotificationDto
                {
                    ShippingAgentRepresentativeId = requestingRepId,
                    VesselId = string.IsNullOrWhiteSpace(vesselId) ? null : new Guid(vesselId),
                    ETA = ParseDateString(periodStart),
                    ETD = ParseDateString(periodEnd),
                    Status = statusString
                };

                var results = await _service.GetForAgentAsync(dto, ct);

                // Aplicar filtros adicionais no controller quando necessários
                if (!string.IsNullOrWhiteSpace(businessId))
                {
                    results = results.Where(r => !string.IsNullOrWhiteSpace(r.BusinessId) &&
                                                 r.BusinessId.Contains(businessId, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (businessIdYear.HasValue)
                {
                    results = results.Where(r => r.CreatedAt.Year == businessIdYear.Value).ToList();
                }

                return Ok(results);
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for search");
                return BadRequest(new { error = "Validation failed", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on search");
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing search");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }


        /// <summary>
        /// Performs an advanced search for vessel visit notifications
        /// </summary>
        [HttpPost("search/advanced")]
        [ProducesResponseType(typeof(List<VesselVisitNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdvancedSearch(
            [FromBody] VesselVisitNotificationDto dto,
            CancellationToken ct = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Validation failed", details = ModelState });
                }

                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int requestingRepId))
                {
                    return Unauthorized(new { error = "User ID not found in claims" });
                }

                // Override requesting representative ID with authenticated user
                dto.ShippingAgentRepresentativeId = requestingRepId;

                var results = await _service.GetForAgentAsync(dto, ct);
                return Ok(results);
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for advanced search");
                return BadRequest(new { error = "Validation failed", message = ex.Message });
            }
            catch (BusinessRuleViolationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on advanced search");
                return BadRequest(new { error = "Business rule violation", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing advanced search");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // Helper methods
        private DateTime? ParseDateString(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return DateTime.TryParse(input, out var date) ? date : null;
        }

        private List<int>? ParseCommaSeparatedInts(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var result = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var val) ? (int?)val : null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            return result.Any() ? result : null;
        }

        private List<VesselNotificationStatus>? ParseStatuses(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var result = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Enum.TryParse<VesselNotificationStatus>(s.Trim(), true, out var status) ? (VesselNotificationStatus?)status : null)
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
                .ToList();

            return result.Any() ? result : null;
        }
    }
}
