
using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Application.Services;

public sealed class ProjectService(
    IProjectRepository projectRepository) : IProjectService
{
    public async Task<IReadOnlyList<Project>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await projectRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Project> CreateAsync(
        string name,
        string client,
        DateRange duration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(duration);
        name = name.Trim();
        client = client.Trim();
        var project = new Project(name, client, duration);

        await projectRepository.AddAsync(project, cancellationToken);
        return project;
    }

    public async Task AddRequiredSkillAsync(
        Guid projectId,
        SkillRequirement requirement,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requirement);

        var project = await projectRepository.GetByIdAsync(
            projectId, cancellationToken)
            ?? throw new InvalidOperationException("Project not found.");

        project.AddOrUpdateSkillRequirement(requirement);

        await projectRepository.UpdateAsync(project, cancellationToken);
    }

    public async Task ChangeStatusAsync(
        Guid projectId,
        ProjectStatus status,
        CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetByIdAsync(
            projectId, cancellationToken)
            ?? throw new InvalidOperationException("Project not found.");

        project.ChangeStatus(status);

        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}
