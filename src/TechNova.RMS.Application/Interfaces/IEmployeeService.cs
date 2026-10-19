
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Application.Interfaces;

public interface IEmployeeService
{
    Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Employee> CreateAsync(
        string name,
        string email,
        string designation,
        string department,
        EmploymentType employmentType,
        decimal costRate,
        CancellationToken cancellationToken = default);

    Task AddSkillAsync(
        Guid employeeId,
        Skill skill,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
