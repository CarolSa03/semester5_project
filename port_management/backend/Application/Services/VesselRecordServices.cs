using PortManagement.Application.Services.IServices;
using PortManagement.Domain.Vessel.Entities;
using PortManagement.Domain.Vessel.ValueObjects;
using PortManagement.Application.Common.Interfaces;

namespace PortManagement.Application.Services
{
    public class VesselRecordService : IVesselRecordService
    {
        private readonly IVesselRecordRepository _repository;
        private readonly IVesselTypeRepository _vesselTypeRepo;

        public VesselRecordService(IVesselRecordRepository repository, IVesselTypeRepository vesselTypeRepo)
        {
            _repository = repository;
            _vesselTypeRepo = vesselTypeRepo;
        }

        public async Task<IEnumerable<VesselRecord>> GetAllAsync(CancellationToken ct = default)
        {
            return await _repository.GetAllAsync(ct);
        }

        public async Task<VesselRecord?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _repository.GetByIdAsync(id, ct);
        }

        public async Task<VesselRecord?> GetByIMOAsync(Imo imo, CancellationToken ct = default)
        {
            return await _repository.GetByIMOAsync(imo, ct);
        }

        public async Task<IEnumerable<VesselRecord>> SearchAsync(string searchTerm, CancellationToken ct = default)
        {
            return await _repository.SearchAsync(searchTerm, ct);
        }

        public async Task AddAsync(VesselRecord vesselRecord, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(vesselRecord);

            var existing = await _repository.GetByIMOAsync(vesselRecord.Imo, ct);
            if (existing != null)
                throw new InvalidOperationException($"Vessel with IMO {vesselRecord.Imo} already exists");

            await _repository.AddAsync(vesselRecord, ct);
            await _repository.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(VesselRecord vesselRecord, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(vesselRecord);

            var existing = await _repository.GetByIdAsync(vesselRecord.Id, ct);
            if (existing == null)
                throw new KeyNotFoundException($"Vessel with Id '{vesselRecord.Id}' not found.");

            await _repository.UpdateAsync(vesselRecord, ct);
            await _repository.SaveChangesAsync(ct);
        }

        public async Task InactivateAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _repository.GetByIdAsync(id, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Vessel with Id '{id}' not found.");

            entity.MarkInactive();
            await _repository.UpdateAsync(entity, ct);
            await _repository.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _repository.GetByIdAsync(id, ct);
            if (entity == null)
                throw new KeyNotFoundException($"Vessel with Id '{id}' not found.");

            await _repository.DeleteAsync(entity, ct);
            await _repository.SaveChangesAsync(ct);
        }
    }
}
