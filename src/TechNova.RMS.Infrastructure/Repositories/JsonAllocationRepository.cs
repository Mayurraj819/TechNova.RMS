using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.ValueObjects;
using TechNova.RMS.Infrastructure.Persistence;
using TechNova.RMS.Infrastructure.Persistence.Models;

namespace TechNova.RMS.Infrastructure.Repositories;

public sealed class JsonAllocationRepository : JsonRepositoryBase<AllocationDto, Allocation>, IAllocationRepository
{
    public JsonAllocationRepository(IJsonFileStore<AllocationDto> store) : base(store)
    {
    }

    protected override AllocationDto ToDto(Allocation allocation) => new()
    {
        Id = allocation.Id,
        EmployeeId = allocation.EmployeeId,
        ProjectId = allocation.ProjectId,
        StartDate = allocation.Duration.Start,
        EndDate = allocation.Duration.End,
        Percentage = allocation.Percentage.Value
    };

    protected override Allocation ToDomain(AllocationDto dto) =>
        Allocation.Reconstitute(
            dto.Id,
            dto.EmployeeId,
            dto.ProjectId,
            new DateRange(dto.StartDate, dto.EndDate),
            new AllocationPercentage(dto.Percentage));

    protected override Guid GetId(AllocationDto dto) => dto.Id;
}
