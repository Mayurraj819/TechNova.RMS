using TechNova.RMS.Domain.Enums;

namespace TechNova.RMS.Infrastructure.Persistence.Models;

public sealed class EmployeeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public decimal CostRate { get; set; }
    public bool IsActive { get; set; }
    public List<SkillDto> Skills { get; set; } = [];
}

public sealed class SkillDto
{
    public string Name { get; set; } = string.Empty;
    public int Proficiency { get; set; }
}