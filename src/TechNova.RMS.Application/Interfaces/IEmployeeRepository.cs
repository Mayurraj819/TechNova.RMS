using TechNova.RMS.Domain.Entities;

namespace TechNova.RMS.Application.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Employee employee,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Employee employee,
        CancellationToken cancellationToken = default);
}