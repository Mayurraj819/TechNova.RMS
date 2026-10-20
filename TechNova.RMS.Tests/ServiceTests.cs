using TechNova.RMS.Application.Services;
using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Tests;

public sealed class ServiceTests
{
    [Fact]
    public async Task EmployeeService_CreateAsync_TrimsValuesAndPersistsEmployee()
    {
        var repository = new InMemoryEmployeeRepository();
        var service = new EmployeeService(repository);

        var employee = await service.CreateAsync(
            "  Ada Lovelace ",
            "  ada@example.com ",
            "Developer",
            "Engineering",
            EmploymentType.FullTime,
            125m);

        Assert.Equal("Ada Lovelace", employee.Name);
        Assert.Equal("ada@example.com", employee.Email);
        Assert.Single(repository.Items);
    }

    [Fact]
    public async Task EmployeeService_CreateAsync_RejectsDuplicateEmailIgnoringCase()
    {
        var repository = new InMemoryEmployeeRepository();
        await repository.AddAsync(CreateEmployee(email: "ada@example.com"));
        var service = new EmployeeService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(
                "Another Employee",
                " ADA@EXAMPLE.COM ",
                "Developer",
                "Engineering",
                EmploymentType.Contract,
                100m));

        Assert.Equal("An employee with this email already exists.", exception.Message);
    }

    [Fact]
    public async Task ProjectService_ChangeStatusAsync_UpdatesProject()
    {
        var project = CreateProject();
        var repository = new InMemoryProjectRepository(project);
        var service = new ProjectService(repository);

        await service.ChangeStatusAsync(project.Id, ProjectStatus.Active);

        Assert.Equal(ProjectStatus.Active, project.Status);
        Assert.Same(project, repository.UpdatedEntity);
    }

    [Fact]
    public async Task AllocationService_AllocateAsync_PersistsAllocationForActiveEmployee()
    {
        var employee = CreateEmployee();
        var project = CreateProject();
        var allocationRepository = new InMemoryAllocationRepository();
        var service = new AllocationService(
            new InMemoryEmployeeRepository(employee),
            new InMemoryProjectRepository(project),
            allocationRepository);

        var allocation = await service.AllocateAsync(
            employee.Id,
            project.Id,
            new DateRange(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 20)),
            new AllocationPercentage(50));

        Assert.Single(allocationRepository.Items);
        Assert.Equal(employee.Id, allocation.EmployeeId);
        Assert.Equal(project.Id, allocation.ProjectId);
        Assert.Equal(50, allocation.Percentage.Value);
    }

    [Fact]
    public async Task AllocationService_AllocateAsync_RejectsInactiveEmployee()
    {
        var employee = CreateEmployee();
        employee.Deactivate();
        var project = CreateProject();
        var service = new AllocationService(
            new InMemoryEmployeeRepository(employee),
            new InMemoryProjectRepository(project),
            new InMemoryAllocationRepository());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AllocateAsync(
                employee.Id,
                project.Id,
                new DateRange(new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 20)),
                new AllocationPercentage(50)));

        Assert.Equal("An inactive employee cannot be allocated.", exception.Message);
    }

    private static Employee CreateEmployee(string email = "employee@example.com") =>
        new("Test Employee", email, "Developer", "Engineering", EmploymentType.FullTime, 100m);

    private static Project CreateProject() =>
        new(
            "Test Project",
            "Test Client",
            new DateRange(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));
}

internal sealed class InMemoryEmployeeRepository(params Employee[] initial) :
    IEmployeeRepository
{
    public List<Employee> Items { get; } = [.. initial];

    public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Employee>>(Items);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.FirstOrDefault(item => item.Id == id));

    public Task AddAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Employee entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Items.RemoveAll(item => item.Id == id);
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryProjectRepository(params Project[] initial) : IProjectRepository
{
    public List<Project> Items { get; } = [.. initial];
    public Project? UpdatedEntity { get; private set; }

    public Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Project>>(Items);

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.FirstOrDefault(item => item.Id == id));

    public Task AddAsync(Project entity, CancellationToken cancellationToken = default)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Project entity, CancellationToken cancellationToken = default)
    {
        UpdatedEntity = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Items.RemoveAll(item => item.Id == id);
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryAllocationRepository(params Allocation[] initial) : IAllocationRepository
{
    public List<Allocation> Items { get; } = [.. initial];

    public Task<IReadOnlyList<Allocation>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Allocation>>(Items);

    public Task<Allocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.FirstOrDefault(item => item.Id == id));

    public Task AddAsync(Allocation entity, CancellationToken cancellationToken = default)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Allocation entity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Items.RemoveAll(item => item.Id == id);
        return Task.CompletedTask;
    }
}
