
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Application.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyList<Project>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Project> CreateAsync(
        string name,
        string client,
        DateRange duration,
        CancellationToken cancellationToken = default);

    Task AddRequiredSkillAsync(
        Guid projectId,
        SkillRequirement requirement,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        Guid projectId,
        ProjectStatus status,
        CancellationToken cancellationToken = default);
}
