
using Microsoft.Extensions.DependencyInjection;
using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Application.Services;
using TechNova.RMS.ConsoleApp;
using TechNova.RMS.Infrastructure.Persistence;
using TechNova.RMS.Infrastructure.Persistence.Models;
using TechNova.RMS.Infrastructure.Repositories;

var services = new ServiceCollection();

var dataDirectory = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        "../../../../..",
        "data"));

Directory.CreateDirectory(dataDirectory);

Console.WriteLine($"JSON data folder: {dataDirectory}");

services.AddSingleton<IJsonFileStore<EmployeeDto>>(
    new JsonFileStore<EmployeeDto>(
        Path.Combine(dataDirectory, "employees.json")));

services.AddSingleton<IJsonFileStore<ProjectDto>>(
    new JsonFileStore<ProjectDto>(
        Path.Combine(dataDirectory, "projects.json")));

services.AddSingleton<IJsonFileStore<AllocationDto>>(
    new JsonFileStore<AllocationDto>(
        Path.Combine(dataDirectory, "allocations.json")));

services.AddSingleton<IEmployeeRepository, JsonEmployeeRepository>();
services.AddSingleton<IProjectRepository, JsonProjectRepository>();
services.AddSingleton<IAllocationRepository, JsonAllocationRepository>();

services.AddScoped<IEmployeeService, EmployeeService>();
services.AddScoped<IProjectService, ProjectService>();
services.AddScoped<IAllocationService, AllocationService>();

services.AddScoped<ConsoleMenu>();

using var serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();

var menu = scope.ServiceProvider.GetRequiredService<ConsoleMenu>();

await menu.RunAsync();
