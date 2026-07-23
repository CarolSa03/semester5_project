using PortManagement.Application.DTOs;
using PortManagement.Domain.Qualification.Entities;

namespace PortManagement.Application.Services.IServices
{
    public interface IQualificationService
    {
        Task<IEnumerable<Qualification>> GetAllQualificationsAsync();
        Task<Qualification?> GetQualificationByIdAsync(Guid id);
        Task<Qualification?> GetQualificationByCodeAsync(string code);
        Task<Qualification?> GetQualificationByNameAsync(string name);
        Task CreateQualificationAsync(Qualification qualification);
        Task UpdateQualificationAsync(Qualification qualification);
        Task DeleteQualificationAsync(string code);
    }
}
