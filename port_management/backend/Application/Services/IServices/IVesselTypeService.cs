using PortManagement.Application.DTOs;

namespace PortManagement.Application.Services.IServices;

public interface IVesselTypeService
{
    Task<List<VesselTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<VesselTypeDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<VesselTypeDto> AddAsync(VesselTypeDto dto, CancellationToken ct = default);
    Task<VesselTypeDto?> UpdateAsync(Guid id, VesselTypeDto dto, CancellationToken ct = default);
    Task<VesselTypeDto?> InactivateAsync(Guid id, CancellationToken ct = default);
    Task<VesselTypeDto?> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<List<VesselTypeDto>> SearchAsync(string searchTerm, CancellationToken ct = default);
    Task<List<VesselTypeDto>> GetActiveAsync(CancellationToken ct = default);
}
