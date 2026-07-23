using System.ComponentModel.DataAnnotations;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.Enums;
using PortManagement.Domain.Vessel.ValueObjects;

namespace PortManagement.Application.DTOs
{
    // Data Transfer Object for cargo manifest information
    public class CargoManifestDto
    {

        public Guid Id { get; set; }
        public Guid VesselVisitNotificationId { get; set; }

        // Type of cargo operation (Loading or Unloading)
        [Required(ErrorMessage = "Manifest type is required")]
        [RegularExpression("Unloading|Loading", ErrorMessage = "Type must be either 'Unloading' or 'Loading'")]
        public string? Type { get; set; } // Type of manifest (Unloading or Loading)

        // List of containers in this manifest
        [Required(ErrorMessage = "Containers list is required")]
        [MinLength(1, ErrorMessage = "At least one container is required")]
        public List<ContainerInfoDto> Containers { get; set; } = new(); // List of containers in the manifest

        public static explicit operator CargoManifest(CargoManifestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new CargoManifest
            {
                Type = ManifestTypeExtensions.ToManifestType(dto.Type),
                Containers = dto.Containers?.Select(c => (ContainerInfo)c).ToList() ?? new()
            };
        }

        public static explicit operator CargoManifestDto(CargoManifest entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new CargoManifestDto
            {
                Type = entity.Type.ToString(),
                Containers = entity.Containers?.Select(c => (ContainerInfoDto)c).ToList() ?? new()
            };
        }

    }

    // Data Transfer Object for container information
    public class ContainerInfoDto
    {
        public Guid Id { get; set; }



        // Container identifier following ISO 6346:2022 standard
        [Required(ErrorMessage = "Container ID is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Container ID must be exactly 11 characters")]
        public string? ContainerId { get; set; } // Unique container ID (11 characters)

        // Description of container contents
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; } // Description of the container's contents

        // Type of cargo (General, Refrigerated, Dangerous, etc.)
        [Required(ErrorMessage = "Cargo type is required")]
        [StringLength(50, ErrorMessage = "Cargo type cannot exceed 50 characters")]
        public string? CargoType { get; set; } // Type of cargo in the container

        // Bay location on the vessel
        [Range(0, 1000, ErrorMessage = "Bay must be between 0 and 1000")]
        public int Bay { get; set; } // Bay position on the vessel

        // Row location on the vessel
        [Range(0, 100, ErrorMessage = "Row must be between 0 and 100")]
        public int Row { get; set; } // Row position on the vessel

        // Tier location on the vessel
        [Range(0, 100, ErrorMessage = "Tier must be between 0 and 100")]
        public int Tier { get; set; } // Tier position on the vessel

        public static explicit operator ContainerInfo(ContainerInfoDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new ContainerInfo
            {
                ContainerId = new CMContainerId(dto.ContainerId!),
                Description = dto.Description,
                CargoType = new CMCargoType(dto.CargoType),
                Bay = dto.Bay,
                Row = dto.Row,
                Tier = dto.Tier
            };
        }

        public static explicit operator ContainerInfoDto(ContainerInfo entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new ContainerInfoDto
            {
                ContainerId = entity.ContainerId.Value,
                Description = entity.Description,
                CargoType = entity.CargoType.Value,
                Bay = entity.Bay,
                Row = entity.Row,
                Tier = entity.Tier
            };
        }
    }

}
