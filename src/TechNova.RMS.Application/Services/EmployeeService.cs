
using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Application.Services;

public sealed class EmployeeService(
    IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await employeeRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Employee> CreateAsync(
        string name,
        string email,
        string designation,
        string department,
        EmploymentType employmentType,
        decimal costRate,
        CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllAsync(
            cancellationToken);
        email = email.Trim();

        if (employees.Any(e =>
                e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "An employee with this email already exists.");
        }

        var employee = new Employee(
            name, email, designation, department,
            employmentType, costRate);

        await employeeRepository.AddAsync(employee, cancellationToken);
        return employee;
    }

    public async Task AddSkillAsync(
        Guid employeeId,
        Skill skill,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(skill);

        var employee = await employeeRepository.GetByIdAsync(
            employeeId, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found.");

        employee.AddOrUpdateSkill(skill);

        await employeeRepository.UpdateAsync(employee, cancellationToken);
    }

    public async Task DeactivateAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(
            employeeId, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found.");

        employee.Deactivate();

        await employeeRepository.UpdateAsync(employee, cancellationToken);
    }
}
