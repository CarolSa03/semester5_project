using PortManagement.Application.DTOs.Auth;
using PortManagement.Domain.Auth;

namespace PortManagement.Application.Mappers
{
    public static class AuthMapper
    {
        public static AppUserDto ToDto(AppUser entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new AppUserDto
            {
                Id = entity.Id,
                IamUserId = entity.IamUserId,
                Email = entity.Email,
                Name = entity.Name,
                IsActive = entity.IsActive,
                ActivatedOn = entity.ActivatedOn,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Roles = entity.GetRoleNames().ToList(),
                ActivationToken = entity.ActivationToken
            };
        }

        public static UserAuthorizationDto ToAuthorizationDto(AppUser entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new UserAuthorizationDto
            {
                UserId = entity.Id,
                Email = entity.Email,
                Name = entity.Name,
                Roles = entity.GetRoles().ToList(),
                IsActive = entity.IsActive
            };
        }

        public static AuditLogDto ToDto(AuditLog entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new AuditLogDto
            {
                Id = entity.Id,
                EventType = entity.EventType.ToString(),
                Details = entity.Details,
                PerformedBy = entity.PerformedBy,
                AffectedUser = entity.AffectedUser,
                IpAddress = entity.IpAddress,
                Timestamp = entity.Timestamp,
                StatusCode = entity.StatusCode
            };
        }
        public static List<AppUserDto> ToDtoList(IEnumerable<AppUser> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            return entities.Select(ToDto).ToList();
        }
        public static List<AuditLogDto> ToDtoList(IEnumerable<AuditLog> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            return entities.Select(ToDto).ToList();
        }
    }
}
