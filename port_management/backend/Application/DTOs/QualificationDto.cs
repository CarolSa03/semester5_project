using PortManagement.Domain.Qualification.Entities;
using PortManagement.Domain.Qualification.ValueObjects;
using PortManagement.Domain.Staff.Entities;

namespace PortManagement.Application.DTOs;

public class QualificationDto
{
    public Guid? Id { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }

    public static explicit operator QualificationDto(Qualification entity)
    {

        return new QualificationDto
        {
            Id = entity.Id,
            Code = entity.Code.Value,
            Description = entity.Description.Value
        };
    }

    public static explicit operator Qualification(QualificationDto dto)
    {

        var code = dto.Code;
        var description = dto.Description;

        return new Qualification(code, description);
    }
}
