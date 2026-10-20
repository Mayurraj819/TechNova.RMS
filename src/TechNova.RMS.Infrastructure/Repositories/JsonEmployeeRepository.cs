using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.ValueObjects;
using TechNova.RMS.Infrastructure.Persistence;
using TechNova.RMS.Infrastructure.Persistence.Models;

namespace TechNova.RMS.Infrastructure.Repositories;

public sealed class JsonEmployeeRepository(
    IJsonFileStore<EmployeeDto> store) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var records = await store.ReadAllAsync(cancellationToken);
        return records.Select(ToDomain).ToList();
    }

    public async Task<Employee?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var records = await store.ReadAllAsync(cancellationToken);
        var record = records.FirstOrDefault(x => x.Id == id);
        return record is null ? null : ToDomain(record);
    }

    public async Task AddAsync(
        Employee employee,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        var records = (await store.ReadAllAsync(cancellationToken)).ToList();

        if (records.Any(x => x.Id == employee.Id))
            throw new InvalidOperationException("Employee already exists.");

        records.Add(ToDto(employee));
        await store.WriteAllAsync(records, cancellationToken);
    }

    public async Task UpdateAsync(
        Employee employee,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        var records = (await store.ReadAllAsync(cancellationToken)).ToList();
        var index = records.FindIndex(x => x.Id == employee.Id);

        if (index < 0)
            throw new KeyNotFoundException("Employee not found.");

        records[index] = ToDto(employee);
        await store.WriteAllAsync(records, cancellationToken);
    }

    private static EmployeeDto ToDto(Employee employee) => new()
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

    private static Employee ToDomain(EmployeeDto dto) =>
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
}