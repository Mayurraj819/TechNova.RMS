using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Application.Interfaces;

public interface IAllocationService
{
    Task<Allocation> AllocateAsync(
        Guid employeeId,
        Guid projectId,
        DateRange duration,
        AllocationPercentage percentage,
        CancellationToken cancellationToken = default);
    Task ReleaseAsync(
    Guid allocationId,
    CancellationToken cancellationToken = default);
}