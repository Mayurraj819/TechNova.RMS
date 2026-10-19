using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Domain.Entities;

public sealed class Allocation
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; }
    public Guid ProjectId { get; }
    public DateRange Duration { get; }
    public AllocationPercentage Percentage { get; }

    public Allocation(
        Guid employeeId,
        Guid projectId,
        DateRange duration,
        AllocationPercentage percentage)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException(
                "Employee ID is required.",
                nameof(employeeId));
        }

        if (projectId == Guid.Empty)
        {
            throw new ArgumentException(
                "Project ID is required.",
                nameof(projectId));
        }

        ArgumentNullException.ThrowIfNull(duration);
        ArgumentNullException.ThrowIfNull(percentage);

        Id = Guid.NewGuid();
        EmployeeId = employeeId;
        ProjectId = projectId;
        Duration = duration;
        Percentage = percentage;
    }
    public static Allocation Reconstitute(
    Guid id,
    Guid employeeId,
    Guid projectId,
    DateRange duration,
    AllocationPercentage percentage)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Allocation ID is required.");

        return new Allocation(employeeId, projectId, duration, percentage)
        {
            Id = id
        };
    }
}