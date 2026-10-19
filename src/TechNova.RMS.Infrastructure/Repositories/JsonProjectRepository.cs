using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.ValueObjects;
using TechNova.RMS.Infrastructure.Persistence;
using TechNova.RMS.Infrastructure.Persistence.Models;

namespace TechNova.RMS.Infrastructure.Repositories;

public sealed class JsonProjectRepository : JsonRepositoryBase<ProjectDto, Project>, IProjectRepository
{
    public JsonProjectRepository(IJsonFileStore<ProjectDto> store) : base(store)
    {
    }

    protected override ProjectDto ToDto(Project project) => new()
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

    protected override Project ToDomain(ProjectDto dto) =>
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

    protected override Guid GetId(ProjectDto dto) => dto.Id;
}
