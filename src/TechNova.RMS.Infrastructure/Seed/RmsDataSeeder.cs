
using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Infrastructure.Seed;

public sealed class RmsDataSeeder(
    IEmployeeService employeeService,
    IProjectService projectService,
    IAllocationService allocationService)
{
    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var existingEmployees = await employeeService.GetAllAsync(
            cancellationToken);

        if (existingEmployees.Count > 0)
        {
            Console.WriteLine("Seed data already exists. Skipping seeding.");
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var projectDuration = new DateRange(
            today.AddDays(-30), today.AddDays(180));

        var employeeData = new[]
        {
            ("Aarav Sharma", "aarav@technova.com", "Senior Developer", "Development", "C#", 5),
            ("Priya Patel", "priya@technova.com", "Developer", "Development", "C#", 4),
            ("Rohan Verma", "rohan@technova.com", "Developer", "Development", "Angular", 4),
            ("Ananya Singh", "ananya@technova.com", "QA Engineer", "QA", "Testing", 5),
            ("Vikram Rao", "vikram@technova.com", "QA Engineer", "QA", "Selenium", 4),
            ("Neha Gupta", "neha@technova.com", "DevOps Engineer", "DevOps", "Azure", 5),
            ("Arjun Mehta", "arjun@technova.com", "DevOps Engineer", "DevOps", "Docker", 4),
            ("Kavya Nair", "kavya@technova.com", "Data Engineer", "Data", "SQL", 5),
            ("Ishaan Joshi", "ishaan@technova.com", "Data Analyst", "Data", "Python", 4),
            ("Meera Iyer", "meera@technova.com", "UI Developer", "UI", "Angular", 5),
            ("Aditya Kulkarni", "aditya@technova.com", "Developer", "Development", "C#", 3),
            ("Sana Khan", "sana@technova.com", "QA Engineer", "QA", "Testing", 3),
            ("Rahul Das", "rahul@technova.com", "Developer", "Development", "SQL", 4),
            ("Pooja Desai", "pooja@technova.com", "UI Developer", "UI", "Angular", 3),
            ("Karan Malhotra", "karan@technova.com", "Intern Developer", "Development", "C#", 2)
        };

        var employees = new List<Employee>();

        foreach (var item in employeeData)
        {
            var employee = await employeeService.CreateAsync(
                item.Item1,
                item.Item2,
                item.Item3,
                item.Item4,
                item.Item3.Contains("Intern", StringComparison.OrdinalIgnoreCase)
                    ? EmploymentType.Intern
                    : EmploymentType.FullTime,
                500m,
                cancellationToken);

            await employeeService.AddSkillAsync(
                employee.Id,
                new Skill(item.Item5, item.Item6),
                cancellationToken);

            employees.Add(employee);
        }

        var projectData = new[]
        {
            ("Customer Portal", "Contoso Ltd", "Angular"),
            ("HR Management System", "Fabrikam Inc", "C#"),
            ("Cloud Migration", "Northwind", "Azure"),
            ("Analytics Platform", "Adventure Works", "SQL"),
            ("Automation Suite", "Tailspin Toys", "Testing")
        };

        var projects = new List<Project>();

        foreach (var item in projectData)
        {
            var project = await projectService.CreateAsync(
                item.Item1,
                item.Item2,
                projectDuration,
                cancellationToken);

            await projectService.AddRequiredSkillAsync(
                project.Id,
                new SkillRequirement(item.Item3, 2),
                cancellationToken);

            await projectService.ChangeStatusAsync(
                project.Id,
                ProjectStatus.Active,
                cancellationToken);

            projects.Add(project);
        }

        // Ten allocations, using different employees so no employee
        // exceeds 100% because of these seed allocations.
        var allocationData = new[]
        {
            (0, 0, 100),
            (1, 0, 100),
            (2, 0, 50),
            (3, 4, 100),
            (4, 4, 50),
            (5, 2, 100),
            (6, 2, 50),
            (7, 3, 100),
            (8, 3, 50),
            (9, 1, 100)
        };

        foreach (var item in allocationData)
        {
            await allocationService.AllocateAsync(
                employees[item.Item1].Id,
                projects[item.Item2].Id,
                projectDuration,
                new AllocationPercentage(item.Item3),
                cancellationToken);
        }

        Console.WriteLine(
            "Seed data created: 15 employees, 5 projects, 10 allocations.");
    }
}
