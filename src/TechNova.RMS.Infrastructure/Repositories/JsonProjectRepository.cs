using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.ValueObjects;
using TechNova.RMS.Infrastructure.Persistence;
using TechNova.RMS.Infrastructure.Persistence.Models;

namespace TechNova.RMS.Infrastructure.Repositories;

public sealed class JsonProjectRepository(
    IJsonFileStore<ProjectDto> store) : IProjectRepository
{
    public async Task<IReadOnlyList<Project>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var records = await store.ReadAllAsync(cancellationToken);
        return records.Select(ToDomain).ToList();
    }

    public async Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var records = await store.ReadAllAsync(cancellationToken);
        var record = records.FirstOrDefault(x => x.Id == id);
        return record is null ? null : ToDomain(record);
    }

    public async Task AddAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        var records = (await store.ReadAllAsync(cancellationToken)).ToList();

        if (records.Any(x => x.Id == project.Id))
            throw new InvalidOperationException("Project already exists.");

        records.Add(ToDto(project));
        await store.WriteAllAsync(records, cancellationToken);
    }

    public async Task UpdateAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        var records = (await store.ReadAllAsync(cancellationToken)).ToList();
        var index = records.FindIndex(x => x.Id == project.Id);

        if (index < 0)
            throw new KeyNotFoundException("Project not found.");

        records[index] = ToDto(project);
        await store.WriteAllAsync(records, cancellationToken);
    }
    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var records = (await store.ReadAllAsync(cancellationToken)).ToList();
        var index = records.FindIndex(item => item.Id == id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"Record with ID '{id}' was not found.");
        }

        records.RemoveAt(index);
        await store.WriteAllAsync(records, cancellationToken);
    }

    private static ProjectDto ToDto(Project project) => new()
    {
        Id = project.Id,
        Name = project.Name,
        Client = project.Client,
        StartDate = project.Duration.Start,
        EndDate = project.Duration.End,
        Status = project.Status,
        RequiredSkills = project.RequiredSkills
            .Select(skill => new SkillRequirementDto
            {
                SkillName = skill.SkillName,
                MinimumProficiency = skill.MinimumProficiency
            })
            .ToList()
    };

    private static Project ToDomain(ProjectDto dto) =>
        Project.Reconstitute(
            dto.Id,
            dto.Name,
            dto.Client,
            new DateRange(dto.StartDate, dto.EndDate),
            dto.Status,
            dto.RequiredSkills.Select(skill =>
                new SkillRequirement(
                    skill.SkillName,
                    skill.MinimumProficiency)));
}