using PortManagement.Application.DTOs;
using PortManagement.Domain.PhysicalResources.Entities;

namespace PortManagement.Application.Mappers;

public static class PhysicalResourceMapper
{
    public static PhysicalResource ToEntity(PhysicalResourceDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return dto switch
        {
            TruckDto truckDto => (Truck)truckDto,
            YardCraneDto yardCraneDto => (YardCrane)yardCraneDto,
            STSCraneDto stsCraneDto => (STSCrane)stsCraneDto,
            _ => throw new ArgumentException("Unknown DTO type", nameof(dto))
        };
    }

    public static PhysicalResourceDto ToDto(PhysicalResource entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return entity switch
        {
            Truck truck => (TruckDto)truck,
            YardCrane yardCrane => (YardCraneDto)yardCrane,
            STSCrane stsCrane => (STSCraneDto)stsCrane,
            _ => throw new ArgumentException("Unknown entity type", nameof(entity))
        };
    }
}
