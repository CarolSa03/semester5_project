using Microsoft.EntityFrameworkCore;
using PortManagement.Application.Common.Interfaces;
using PortManagement.Application.Services.IServices;
using PortManagement.Application.Services;
using PortManagement.Infrastructure.Data;
using PortManagement.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using PortManagement.Api.Middleware;
using PortManagement.Application.Mappers;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultPostgres");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultPostgres' is not configured.");
}

builder.Services.AddDbContext<PortManagementDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<DockMapper>();
builder.Services.AddScoped<IDockRepository, DockRepository>();
builder.Services.AddScoped<IPhysicalResourceRepository, PhysicalResourceRepository>();
builder.Services.AddScoped<IVesselTypeRepository, VesselTypeRepository>();
builder.Services.AddScoped<IQualificationRepository, QualificationRepository>();
builder.Services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
builder.Services.AddScoped<IStorageAreaRepository, StorageAreaRepository>();
builder.Services.AddScoped<IVesselRecordRepository, VesselRecordRepository>();
builder.Services.AddScoped<IVesselVisitNotificationRepository, VesselVisitNotificationRepository>();
builder.Services.AddScoped<IShippingAgentRepresentativeRepository, ShippingAgentRepresentativeRepository>();
builder.Services.AddScoped<IShippingAgentOrganizationRepository, ShippingAgentOrganizationRepository>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IPrivacyPolicyRepository, PrivacyPolicyRepository>();
builder.Services.AddScoped<IPrivacyPolicyService, PrivacyPolicyService>();
builder.Services.AddScoped<IDockService, DockService>();
builder.Services.AddScoped<IPhysicalResourceService, PhysicalResourceService>();
builder.Services.AddScoped<IVesselTypeService, VesselTypeService>();
builder.Services.AddScoped<IQualificationService, QualificationService>();
builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
builder.Services.AddScoped<IStorageAreaService, StorageAreaServices>();
builder.Services.AddScoped<IVesselRecordService, VesselRecordService>();
builder.Services.AddScoped<IShippingAgentRepresentativeService, ShippingAgentRepresentativeService>();
builder.Services.AddScoped<IShippingAgentOrganizationService, ShippingAgentOrganizationService>();
builder.Services.AddScoped<IVesselVisitNotificationService>(sp =>
    new VesselVisitNotificationService(
        sp.GetRequiredService<IVesselVisitNotificationRepository>(),
        sp.GetRequiredService<IShippingAgentRepresentativeRepository>(),
        sp.GetRequiredService<IVesselRecordRepository>(),
        sp.GetRequiredService<IDockRepository>(),
        "DPC"
    )
);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "PortManagement.Auth";
        options.Cookie.HttpOnly = true;

        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"]
            ?? throw new InvalidOperationException("Google:ClientId is not configured");

        options.ClientSecret = builder.Configuration["Google:ClientSecret"]
            ?? throw new InvalidOperationException("Google:ClientSecret is not configured");

        options.SaveTokens = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministrator", policy =>
        policy.RequireRole("Administrator"));

    options.AddPolicy("RequirePortAuthorityOfficer", policy =>
        policy.RequireRole("PortAuthorityOfficer"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Port Management API",
        Version = "v1",
        Description = "Port Management System API with Google OAuth"
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Port Management API v1");
    options.RoutePrefix = "swagger";
});

app.Use(async (context, next) =>
{
    try { await next.Invoke(); }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
});

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders();

app.UseStaticFiles();

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuthorizationLoggingMiddleware>();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403)
    {
        var auditService = context.RequestServices.GetService<IAuditService>();
        if (auditService != null)
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                      ?? context.User.FindFirst("sub")?.Value
                      ?? "Anonymous";

            var resource = context.Request.Path.Value;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            await auditService.LogUnauthorizedAccessAsync(userId, resource, ipAddress);
        }
    }
});

app.MapControllers();

app.Run();
