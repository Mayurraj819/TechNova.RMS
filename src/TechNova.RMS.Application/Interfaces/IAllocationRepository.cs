using TechNova.RMS.Domain.Entities;

namespace TechNova.RMS.Application.Interfaces;

public interface IAllocationRepository :IRepository<Allocation>
{
    Task<IReadOnlyList<Allocation>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Allocation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Allocation allocation,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Allocation allocation,
        CancellationToken cancellationToken = default);
  

    Task DeleteAsync(
        Guid allocationId,
        CancellationToken cancellationToken = default);
}