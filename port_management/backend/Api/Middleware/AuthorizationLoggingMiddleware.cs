using Microsoft.AspNetCore.Http;
using PortManagement.Application.Services.IServices;
using System.Security.Claims;

namespace PortManagement.Api.Middleware
{
    public class AuthorizationLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                var endpoint = context.Request.Path.ToString();

                await auditService.LogUnauthorizedAccessAsync(
                    userId,
                    endpoint,
                    context.Connection.RemoteIpAddress?.ToString()
                );
            }
        }

    }
}
