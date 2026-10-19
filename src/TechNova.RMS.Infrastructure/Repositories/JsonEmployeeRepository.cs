using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.ValueObjects;
using TechNova.RMS.Infrastructure.Persistence;
using TechNova.RMS.Infrastructure.Persistence.Models;

namespace TechNova.RMS.Infrastructure.Repositories;

public sealed class JsonEmployeeRepository : JsonRepositoryBase<EmployeeDto, Employee>, IEmployeeRepository
{
    public JsonEmployeeRepository(IJsonFileStore<EmployeeDto> store) : base(store)
    {
    }

    protected override EmployeeDto ToDto(Employee employee) => new()
    {
        Id = employee.Id,
        Name = employee.Name,
        Email = employee.Email,
        Designation = employee.Designation,
        Department = employee.Department,
        EmploymentType = employee.EmploymentType,
        CostRate = employee.CostRate,
        IsActive = employee.IsActive,
        Skills = employee.Skills.Select(skill => new SkillDto
        {
            Name = skill.Name,
            Proficiency = skill.Proficiency
        }).ToList()
    };

    protected override Employee ToDomain(EmployeeDto dto) =>
        Employee.Reconstitute(
            dto.Id,
            dto.Name,
            dto.Email,
            dto.Designation,
            dto.Department,
            dto.EmploymentType,
            dto.CostRate,
            dto.IsActive,
            dto.Skills.Select(skill =>
                new Skill(skill.Name, skill.Proficiency)));

    protected override Guid GetId(EmployeeDto dto) => dto.Id;
}
