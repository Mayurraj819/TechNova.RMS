
using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.ValueObjects;
using TechNova.RMS.Infrastructure.Persistence;
using TechNova.RMS.Infrastructure.Persistence.Models;

namespace TechNova.RMS.Infrastructure.Repositories;

public sealed class JsonAllocationRepository(
    IJsonFileStore<AllocationDto> store)
    : IAllocationRepository
{
    public async Task<IReadOnlyList<Allocation>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var records = await store.ReadAllAsync(cancellationToken);

        return records.Select(ToDomain).ToList();
    }

    public async Task<Allocation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var records = await store.ReadAllAsync(cancellationToken);
        var dto = records.FirstOrDefault(item => item.Id == id);

        return dto is null ? null : ToDomain(dto);
    }

    public async Task AddAsync(
        Allocation allocation,
        CancellationToken cancellationToken = default)
    {
        var records = (await store.ReadAllAsync(cancellationToken)).ToList();

        if (records.Any(item => item.Id == allocation.Id))
        {
            throw new InvalidOperationException(
                $"Allocation '{allocation.Id}' already exists.");
        }

        records.Add(ToDto(allocation));

        await store.WriteAllAsync(records, cancellationToken);
    }

    public async Task UpdateAsync(
        Allocation allocation,
        CancellationToken cancellationToken = default)
    {
        var records = (await store.ReadAllAsync(cancellationToken)).ToList();
        var index = records.FindIndex(item => item.Id == allocation.Id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"Allocation '{allocation.Id}' was not found.");
        }

        records[index] = ToDto(allocation);

        await store.WriteAllAsync(records, cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var records = (await store.ReadAllAsync(cancellationToken)).ToList();
        var index = records.FindIndex(item => item.Id == id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"Allocation '{id}' was not found.");
        }

        records.RemoveAt(index);

        await store.WriteAllAsync(records, cancellationToken);
    }

    private static Allocation ToDomain(AllocationDto dto)
    {
        return Allocation.Reconstitute(
            dto.Id,
            dto.EmployeeId,
            dto.ProjectId,
            new DateRange(dto.StartDate, dto.EndDate),
            new AllocationPercentage(dto.Percentage));
    }

    private static AllocationDto ToDto(Allocation allocation)
    {
        return new AllocationDto
        {
            Id = allocation.Id,
            EmployeeId = allocation.EmployeeId,
            ProjectId = allocation.ProjectId,
            StartDate = allocation.Duration.Start,
            EndDate = allocation.Duration.End,
            Percentage = allocation.Percentage.Value
        };
    }
}
