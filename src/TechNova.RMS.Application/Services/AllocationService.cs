using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Application.Services;

public sealed class AllocationService(
    IEmployeeRepository employeeRepository,
    IProjectRepository projectRepository,
    IAllocationRepository allocationRepository)
    : IAllocationService
{
    public async Task<Allocation> AllocateAsync(
        Guid employeeId,
        Guid projectId,
        DateRange duration,
        AllocationPercentage percentage,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(duration);
        ArgumentNullException.ThrowIfNull(percentage);

        var employee = await employeeRepository.GetByIdAsync(
            employeeId, cancellationToken);

        if (employee is null)
            throw new InvalidOperationException("Employee not found.");

        if (!employee.IsActive)
            throw new InvalidOperationException(
                "An inactive employee cannot be allocated.");

        var project = await projectRepository.GetByIdAsync(
            projectId, cancellationToken);

        if (project is null)
            throw new InvalidOperationException("Project not found.");

        // Replace Contains(DateOnly) with explicit boundary checks
        if (!(project.Duration.Start <= duration.Start &&
              project.Duration.End >= duration.Start) ||
            !(project.Duration.Start <= duration.End &&
              project.Duration.End >= duration.End))
        {
            throw new InvalidOperationException(
                "Allocation dates must fall within the project dates.");
        }

        var existingAllocations =
            await allocationRepository.GetAllAsync(cancellationToken);

        var employeeAllocations = existingAllocations
            .Where(a => a.EmployeeId == employeeId &&
                        a.Duration.Overlaps(duration))
            .ToList();

        // Check the requested start date and every existing allocation
        // start date within the requested period.
        var checkDates = employeeAllocations
            .Where(a => a.Duration.Start >= duration.Start &&
                        a.Duration.Start <= duration.End)
            .Select(a => a.Duration.Start)
            .Append(duration.Start)
            .Distinct();

        foreach (var date in checkDates)
        {
            // Replace Contains(DateOnly) with explicit boundary check
            var existingPercentage = employeeAllocations
                .Where(a => a.Duration.Start <= date && a.Duration.End >= date)
                .Sum(a => a.Percentage.Value);

            if (existingPercentage + percentage.Value > 100)
            {
                throw new InvalidOperationException(
                    $"Allocation exceeds 100% on {date:yyyy-MM-dd}. " +
                    $"Existing allocation: {existingPercentage}%.");
            }
        }

        var allocation = new Allocation(
            employeeId,
            projectId,
            duration,
            percentage);

        await allocationRepository.AddAsync(
            allocation, cancellationToken);

        return allocation;
    }
    public async Task ReleaseAsync(
    Guid allocationId,
    CancellationToken cancellationToken = default)
    {
        var allocation = await allocationRepository.GetByIdAsync(
            allocationId, cancellationToken)
            ?? throw new InvalidOperationException(
                "Allocation not found.");

        await allocationRepository.DeleteAsync(
            allocationId, cancellationToken);
    }
}