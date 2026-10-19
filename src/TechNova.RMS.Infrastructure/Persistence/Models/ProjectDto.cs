using TechNova.RMS.Domain.Enums;

namespace TechNova.RMS.Infrastructure.Persistence.Models;

public sealed class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public List<SkillRequirementDto> RequiredSkills { get; set; } = [];
}

public sealed class SkillRequirementDto
{
    public string SkillName { get; set; } = string.Empty;
    public int MinimumProficiency { get; set; }
}