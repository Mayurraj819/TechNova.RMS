using TechNova.RMS.Domain.Entities;

namespace TechNova.RMS.Application.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<IReadOnlyList<Project>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Project project,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Project project,
        CancellationToken cancellationToken = default);
}