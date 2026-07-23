using PortManagement.Domain.Qualification.Entities;

namespace PortManagement.Application.Common.Interfaces;

public interface IQualificationRepository
{
    Task AddAsync(Qualification qualification);

    Task UpdateAsync(Qualification qualification);

    Task<Qualification> GetByIdAsync(Guid id);
    Task<Qualification> GetByCodeAsync(string code);
    Task<Qualification> GetByNameAsync(string name);
    Task<List<Qualification>> GetAllAsync();
    Task DeleteAsync(Qualification qualification);
}
