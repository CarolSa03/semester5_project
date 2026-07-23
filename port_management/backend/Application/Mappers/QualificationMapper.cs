using PortManagement.Application.DTOs;
using PortManagement.Domain.Qualification.Entities;
using PortManagement.Domain.Staff.Entities;

namespace PortManagement.Application.Mappers;

public class QualificationMapper
{
    public static QualificationDto ToDto(Qualification entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        return entity switch
        {
            Qualification qualification => (QualificationDto) qualification,
            _ => throw new ArgumentException("Unknown entity type", nameof(entity))
        };
    }

    public static Qualification ToEntity(QualificationDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        return dto switch
        {
            QualificationDto qualificationDto => (Qualification)qualificationDto,
            _ => throw new ArgumentException("Unknown DTO type", nameof(dto))
        };
    }
}
