using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortManagement.Application.DTOs.Auth;
using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Auth;
using System.Security.Claims;

namespace PortManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
//[Authorize(Roles = "Administrator")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAuditService _auditService;
    private readonly List<string> _adminEmails;

    public AuthController(
        IAuthService authService,
        IAuditService auditService,
        IConfiguration configuration)
    {
        _authService = authService;
        _auditService = auditService;
        _adminEmails = configuration.GetSection("AdminEmails").Get<List<string>>() ?? new List<string>();
    }

    [HttpPost("login-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppUserDto>> LoginWithEmail([FromBody] EmailLoginDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new { error = "Email is required" });
            }
            if (!IsValidEmail(dto.Email))
            {
                return BadRequest(new { error = "Invalid email format" });
            }

            var existingUser = await _authService.GetUserByEmailAsync(dto.Email);

            AppUserDto user;
            Guid userId;

            if (existingUser == null)
            {
                var createDto = new CreateAppUserDto
                {
                    IamUserId = Guid.NewGuid().ToString(),
                    Email = dto.Email,
                    Name = dto.Email.Split('@')[0]
                };

                var newUser = await _authService.CreateUserForActivationAsync(createDto, "email-login");
                await _authService.ActivateUserAccountAsync(newUser.Id, "email-login");

                userId = newUser.Id;
                user = newUser;

                await _auditService.LogUserManagementEventAsync(
                    AuditEventType.UserCreated,
                    "email-login",
                    createDto.IamUserId,
                    $"User {dto.Email} created via email login"
                );
            }
            else
            {
                userId = existingUser.Id;
                user = existingUser;

                if (!existingUser.IsActive)
                {
                    await _authService.ActivateUserAccountAsync(userId, "email-login");
                }
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal
            );

            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out" });
    }

    // Helper method to validate email format
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    [HttpGet("user")]
    [Authorize]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AppUserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(Guid.Parse(userIdClaim));
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }

// -------------------------------------------------------------------------

#if DEBUG
  [HttpGet("dev-login/{email}/{roleName}")]
  [AllowAnonymous]
  public async Task<IActionResult> DevLogin(string email, string roleName)
  {
      // 1. Parse the Role String to Enum (e.g., "Administrator" -> Role.Administrator)
      if (!Enum.TryParse<Role>(roleName, true, out var roleEnum))
      {
          return BadRequest($"Invalid Role: '{roleName}'. Available roles usually are: Administrator, PortAuthorityOfficer, LogisticsOperator, etc.");
      }

      // 2. Try to find the user
      var userDto = await _authService.GetUserByEmailAsync(email);

      if (userDto == null)
      {
          // --- CREATE NEW USER ---
          var createDto = new CreateAppUserDto
          {
              IamUserId = Guid.NewGuid().ToString(), // Fake IAM ID
              Email = email,
              Name = email.Split('@')[0]
          };

          // A. Create the base user
          userDto = await _authService.CreateUserForActivationAsync(createDto, "dev-tool");
          
          // B. Force Activation (skip email token)
          await _authService.ActivateUserAccountAsync(userDto.Id, "dev-tool");

          // C. Assign the requested Role
          userDto = await _authService.AssignRoleToUserAsync(userDto.Id, roleEnum, "dev-tool");
      }
      else
      {
          // --- UPDATE EXISTING USER ---
          // If user exists but is not active, activate them
          if (!userDto.IsActive)
          {
              await _authService.ActivateUserAccountAsync(userDto.Id, "dev-tool");
          }

          // Ensure they have the requested role. If not, add it.
          if (!userDto.Roles.Contains(roleEnum.ToString()))
          {
              await _authService.AssignRoleToUserAsync(userDto.Id, roleEnum, "dev-tool");
              // Refresh DTO to get updated roles
              userDto = await _authService.GetUserByIdAsync(userDto.Id);
          }
      }

      // 3. Build Claims for the Cookie
      var claims = new List<Claim>
      {
          new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
          new Claim(ClaimTypes.Email, userDto.Email),
          new Claim(ClaimTypes.Name, userDto.Name),
          // Important: Add the IAM ID as "sub" because some middleware looks for it
          new Claim("sub", userDto.IamUserId) 
      };

      // Add ALL user roles to the cookie
      foreach (var r in userDto.Roles)
      {
          claims.Add(new Claim(ClaimTypes.Role, r));
      }

      var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

      // 4. Create the Session Cookie
      await HttpContext.SignInAsync(
          CookieAuthenticationDefaults.AuthenticationScheme,
          claimsPrincipal,
          new AuthenticationProperties 
          { 
              IsPersistent = true,
              ExpiresUtc = DateTime.UtcNow.AddDays(7)
          }
      );

      return Ok(new 
      { 
          message = $"Login Successful. Cookie Set.",
          user = userDto.Name, 
          roles = userDto.Roles,
          action = "Go back to your frontend or test endpoints now."
      });
  }
#endif

// -------------------------------------------------------------------------

}
public class EmailLoginDto
{
    public string Email { get; set; }
}
