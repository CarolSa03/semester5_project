using PortManagement.Application.DTOs;
using PortManagement.Domain.Vessel.Entities;

namespace PortManagement.Application.Mappers
{
    public static class VesselVisitNotificationMapper
    {
        public static VesselVisitNotificationDto ToDto(VesselVisitNotification entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity switch
            {
                VesselVisitNotification vesselVisitNotification => (VesselVisitNotificationDto)vesselVisitNotification,
                _ => throw new ArgumentException("Unknown entity type", nameof(entity))
            };
        }

        public static CargoManifestDto CargoToDto(CargoManifest entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity switch
            {

            };
        }

        public static CargoManifest CargoToEntity(CargoManifestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return dto switch
            {

            };
        }

        public static CrewInfoDto CrewToDto(CrewInfo entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity switch
            {

            };
        }

        public static CrewInfo CrewToEntity(CrewInfoDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return dto switch
            {

            };
        }
    }
}
