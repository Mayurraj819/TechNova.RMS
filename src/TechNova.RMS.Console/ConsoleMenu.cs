//using TechNova.RMS.Application.Interfaces;

//namespace TechNova.RMS.ConsoleApp;

//public sealed class ConsoleMenu(
//    IEmployeeService employeeService,
//    IProjectService projectService,
//    IAllocationRepository allocationRepository)
//{
//    public async Task RunAsync()
//    {
//        var exit = false;

//        while (!exit)
//        {
//            Console.WriteLine("\n=== TechNova Resource Management System ===");

//            Console.WriteLine("\n=== TechNova Resource Management System ===
//            Console.WriteLine("1. List employees");
//            Console.WriteLine("2. List projects");
//            Console.WriteLine("3. View allocations");
//            Console.WriteLine("4. View bench employees");
//            Console.WriteLine("5. Create employee");
//            Console.WriteLine("6. Add skill to employee");
//            Console.WriteLine("7. Deactivate employee");
//            Console.WriteLine("8. Create project");
//            Console.WriteLine("9. Add required skill to project");
//            Console.WriteLine("10. Change project status");
//            Console.WriteLine("11. Allocate employee to project");
//            Console.WriteLine("12. Release allocation");
//            Console.WriteLine("13. Suggest employees for project");
//            Console.WriteLine("14. Staffing summary report");
//            Console.WriteLine("15. Department utilization report");
//            Console.WriteLine("0. Exit");
//            Console.Write("Select an option: ");


//            switch (Console.ReadLine()?.Trim())
//            {
//                case "1":
//                    await ListEmployeesAsync();
//                    break;
//                case "2":
//                    await ListProjectsAsync();
//                    break;
//                case "3":
//                    await ListAllocationsAsync();
//                    break;
//                case "4":
//                    await ListBenchEmployeesAsync();
//                    break;
//                case "0":
//                    exit = true;
//                    break;
//                default:
//                    Console.WriteLine("Invalid option. Try again.");
//                    break;
//            }
//        }
//    }

//    private async Task ListEmployeesAsync()
//    {
//        var employees = await employeeService.GetAllAsync();

//        Console.WriteLine("\n--- Employees ---");

//        foreach (var employee in employees)
//        {
//            Console.WriteLine(
//                $"{employee.Id} | {employee.Name} | " +
//                $"{employee.Department} | Active: {employee.IsActive}");
//        }

//        Console.WriteLine($"Total employees: {employees.Count}");
//    }

//    private async Task ListProjectsAsync()
//    {
//        var projects = await projectService.GetAllAsync();

//        Console.WriteLine("\n--- Projects ---");

//        foreach (var project in projects)
//        {
//            Console.WriteLine(
//                $"{project.Id} | {project.Name} | {project.Client} | " +
//                $"{project.Status} | " +
//                $"{project.Duration.Start:yyyy-MM-dd} to " +
//                $"{project.Duration.End:yyyy-MM-dd}");
//        }

//        Console.WriteLine($"Total projects: {projects.Count}");
//    }

//    private async Task ListAllocationsAsync()
//    {
//        var allocations = await allocationRepository.GetAllAsync();

//        Console.WriteLine("\n--- Allocations ---");

//        foreach (var allocation in allocations)
//        {
//            Console.WriteLine(
//                $"Employee: {allocation.EmployeeId} | " +
//                $"Project: {allocation.ProjectId} | " +
//                $"{allocation.Percentage.Value}% | " +
//                $"{allocation.Duration.Start:yyyy-MM-dd} to " +
//                $"{allocation.Duration.End:yyyy-MM-dd}");
//        }

//        Console.WriteLine($"Total allocations: {allocations.Count}");
//    }

//    private async Task ListBenchEmployeesAsync()
//    {
//        var employees = await employeeService.GetAllAsync();
//        var allocations = await allocationRepository.GetAllAsync();
//        var today = DateOnly.FromDateTime(DateTime.Today);

//        var benchEmployees = employees
//            .Where(employee => employee.IsActive)
//            .Where(employee => !allocations.Any(allocation =>
//                allocation.EmployeeId == employee.Id &&
//                allocation.Duration.Start <= today &&
//                allocation.Duration.End >= today &&
//                allocation.Percentage.Value > 0))
//            .ToList();

//        Console.WriteLine("\n--- Bench Employees ---");

//        foreach (var employee in benchEmployees)
//        {
//            Console.WriteLine(
//                $"{employee.Name} | {employee.Department}");
//        }

//        Console.WriteLine($"Bench employees: {benchEmployees.Count}");
//    }
//}

using System.Globalization;
using TechNova.RMS.Application.Interfaces;
using TechNova.RMS.Domain.Entities;
using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.ConsoleApp;

public sealed class ConsoleMenu(
    IEmployeeService employeeService,
    IProjectService projectService,
    IAllocationRepository allocationRepository,
    IAllocationService allocationService)
{
    public async Task RunAsync()
    {
        var exit = false;

        while (!exit)
        {
            Console.WriteLine("\n=== TechNova Resource Management System ===");
            Console.WriteLine("1. List employees");
            Console.WriteLine("2. List projects");
            Console.WriteLine("3. View allocations");
            Console.WriteLine("4. View bench employees");
            Console.WriteLine("5. Create employee");
            Console.WriteLine("6. Add skill to employee");
            Console.WriteLine("7. Deactivate employee");
            Console.WriteLine("8. Create project");
            Console.WriteLine("9. Add required skill to project");
            Console.WriteLine("10. Change project status");
            Console.WriteLine("11. Allocate employee to project");
            Console.WriteLine("12. Release allocation");
            Console.WriteLine("13. Suggest employees for project");
            Console.WriteLine("14. Staffing summary report");
            Console.WriteLine("15. Department utilization report");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            try
            {
                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        await ListEmployeesAsync();
                        break;
                    case "2":
                        await ListProjectsAsync();
                        break;
                    case "3":
                        await ListAllocationsAsync();
                        break;
                    case "4":
                        await ListBenchEmployeesAsync();
                        break;
                    case "5":
                        await CreateEmployeeAsync();
                        break;
                    case "6":
                        await AddEmployeeSkillAsync();
                        break;
                    case "7":
                        await DeactivateEmployeeAsync();
                        break;
                    case "8":
                        await CreateProjectAsync();
                        break;
                    case "9":
                        await AddProjectSkillAsync();
                        break;
                    case "10":
                        await ChangeProjectStatusAsync();
                        break;
                    case "11":
                        await AllocateEmployeeAsync();
                        break;
                    case "12":
                        await ReleaseAllocationAsync();
                        break;
                    case "13":
                        await SuggestEmployeesAsync();
                        break;
                    case "14":
                        await StaffingSummaryAsync();
                        break;
                    case "15":
                        await DepartmentUtilizationAsync();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failed: {ex.Message}");
            }
        }

        Console.WriteLine("Exiting TechNova RMS.");
    }

    private async Task ListEmployeesAsync()
    {
        var employees = await employeeService.GetAllAsync();

        Console.WriteLine("\n--- Employees ---");

        foreach (var employee in employees)
        {
            Console.WriteLine(
                $"{employee.Id} | {employee.Name} | " +
                $"{employee.Department} | {employee.Designation} | " +
                $"Active: {employee.IsActive}");
        }

        Console.WriteLine($"Total employees: {employees.Count}");
    }

    private async Task ListProjectsAsync()
    {
        var projects = await projectService.GetAllAsync();

        Console.WriteLine("\n--- Projects ---");

        foreach (var project in projects)
        {
            Console.WriteLine(
                $"{project.Id} | {project.Name} | {project.Client} | " +
                $"{project.Status} | " +
                $"{project.Duration.Start:yyyy-MM-dd} to " +
                $"{project.Duration.End:yyyy-MM-dd}");
        }

        Console.WriteLine($"Total projects: {projects.Count}");
    }

    private async Task ListAllocationsAsync()
    {
        var allocations = await allocationRepository.GetAllAsync();

        Console.WriteLine("\n--- Allocations ---");

        foreach (var allocation in allocations)
        {
            Console.WriteLine(
                $"{allocation.Id} | Employee: {allocation.EmployeeId} | " +
                $"Project: {allocation.ProjectId} | " +
                $"{allocation.Percentage.Value}% | " +
                $"{allocation.Duration.Start:yyyy-MM-dd} to " +
                $"{allocation.Duration.End:yyyy-MM-dd}");
        }

        Console.WriteLine($"Total allocations: {allocations.Count}");
    }

    private async Task ListBenchEmployeesAsync()
    {
        var employees = await employeeService.GetAllAsync();
        var allocations = await allocationRepository.GetAllAsync();
        var today = DateOnly.FromDateTime(DateTime.Today);

        var benchEmployees = employees
            .Where(e => e.IsActive)
            .Where(e => !allocations.Any(a =>
                a.EmployeeId == e.Id &&
                a.Duration.Start <= today &&
                a.Duration.End >= today &&
                a.Percentage.Value > 0))
            .ToList();

        Console.WriteLine("\n--- Bench Employees ---");

        foreach (var employee in benchEmployees)
        {
            Console.WriteLine($"{employee.Name} | {employee.Department}");
        }

        Console.WriteLine($"Bench employees: {benchEmployees.Count}");
    }

    private async Task CreateEmployeeAsync()
    {
        Console.WriteLine("\n--- Create Employee ---");

        var name = ReadRequired("Name: ");
        var email = ReadRequired("Email: ");
        var designation = ReadRequired("Designation: ");
        var department = ReadRequired("Department: ");

        var employmentType = ReadEnum<EmploymentType>("Employment type");
        var costRate = ReadDecimal("Cost rate: ");

        var employee = await employeeService.CreateAsync(
            name,
            email,
            designation,
            department,
            employmentType,
            costRate);

        Console.WriteLine($"Employee created successfully. ID: {employee.Id}");
    }

    private async Task AddEmployeeSkillAsync()
    {
        Console.WriteLine(
            "Skill creation depends on the Skill value-object constructor " +
            "and its proficiency model in your Domain project.");
        Console.WriteLine(
            "Connect this menu operation to employeeService.AddSkillAsync " +
            "using your existing Skill constructor.");
        await Task.CompletedTask;
    }

    private async Task DeactivateEmployeeAsync()
    {
        var employeeId = ReadGuid("Employee ID to deactivate: ");

        await employeeService.DeactivateAsync(employeeId);

        Console.WriteLine("Employee deactivated successfully.");
    }

    private async Task CreateProjectAsync()
    {
        Console.WriteLine("\n--- Create Project ---");

        var name = ReadRequired("Project name: ");
        var client = ReadRequired("Client name: ");
        var start = ReadDate("Start date (yyyy-MM-dd): ");
        var end = ReadDate("End date (yyyy-MM-dd): ");

        if (end < start)
        {
            Console.WriteLine("End date cannot be earlier than start date.");
            return;
        }

        var duration = new DateRange(start, end);
        var project = await projectService.CreateAsync(name, client, duration);

        Console.WriteLine($"Project created successfully. ID: {project.Id}");
    }

    private async Task AddProjectSkillAsync()
    {
        Console.WriteLine(
            "Skill requirement creation depends on the SkillRequirement " +
            "constructor and proficiency model in your Domain project.");
        Console.WriteLine(
            "Connect this menu operation to projectService.AddRequiredSkillAsync " +
            "using your existing SkillRequirement constructor.");
        await Task.CompletedTask;
    }

    private async Task ChangeProjectStatusAsync()
    {
        var projectId = ReadGuid("Project ID: ");
        var status = ReadEnum<ProjectStatus>("Project status");

        await projectService.ChangeStatusAsync(projectId, status);

        Console.WriteLine("Project status updated successfully.");
    }

    private async Task AllocateEmployeeAsync()
    {
        Console.WriteLine("\n--- Allocate Employee ---");

        var employeeId = ReadGuid("Employee ID: ");
        var projectId = ReadGuid("Project ID: ");
        var start = ReadDate("Allocation start date (yyyy-MM-dd): ");
        var end = ReadDate("Allocation end date (yyyy-MM-dd): ");
        var percentage = ReadInt("Allocation percentage (1-100): ");

        if (end < start)
        {
            Console.WriteLine("End date cannot be earlier than start date.");
            return;
        }

        if (percentage < 1 || percentage > 100)
        {
            Console.WriteLine("Allocation percentage must be between 1 and 100.");
            return;
        }

        var duration = new DateRange(start, end);
        var allocationPercentage = new AllocationPercentage(percentage);

        var allocation = await allocationService.AllocateAsync(
            employeeId,
            projectId,
            duration,
            allocationPercentage);

        Console.WriteLine($"Allocation created successfully. ID: {allocation.Id}");
    }

    private async Task ReleaseAllocationAsync()
    {
        Console.WriteLine(
            "Release is not available through the current IAllocationService. " +
            "Add a ReleaseAsync method and repository removal/update operation " +
            "before enabling this action.");
        await Task.CompletedTask;
    }

    private async Task SuggestEmployeesAsync()
    {
        var projectId = ReadGuid("Project ID: ");

        var employees = await employeeService.GetAllAsync();
        var projects = await projectService.GetAllAsync();
        var allocations = await allocationRepository.GetAllAsync();

        var project = projects.FirstOrDefault(p => p.Id == projectId);

        if (project is null)
        {
            Console.WriteLine("Project not found.");
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        var candidates = employees
            .Where(e => e.IsActive)
            .Where(e => !allocations.Any(a =>
                a.EmployeeId == e.Id &&
                a.Duration.Start <= today &&
                a.Duration.End >= today &&
                a.Percentage.Value >= 100))
            .ToList();

        Console.WriteLine($"\n--- Employee Suggestions: {project.Name} ---");

        if (candidates.Count == 0)
        {
            Console.WriteLine("No matching active employees found.");
            return;
        }

        foreach (var employee in candidates)
        {
            var currentAllocation = allocations
                .Where(a => a.EmployeeId == employee.Id &&
                            a.Duration.Start <= today &&
                            a.Duration.End >= today)
                .Sum(a => a.Percentage.Value);

            Console.WriteLine(
                $"{employee.Name} | {employee.Department} | " +
                $"Current allocation: {currentAllocation}%");
        }

        Console.WriteLine(
            "Note: skill-based matching requires comparing the project's " +
            "required skills with each employee's skills.");
    }

    private async Task StaffingSummaryAsync()
    {
        var employees = await employeeService.GetAllAsync();
        var projects = await projectService.GetAllAsync();
        var allocations = await allocationRepository.GetAllAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        var activeEmployees = employees.Where(e => e.IsActive).ToList();

        var allocatedEmployeeIds = allocations
            .Where(a => a.Duration.Start <= today &&
                        a.Duration.End >= today &&
                        a.Percentage.Value > 0)
            .Select(a => a.EmployeeId)
            .Distinct()
            .ToHashSet();

        Console.WriteLine("\n--- Staffing Summary ---");
        Console.WriteLine($"Total employees: {employees.Count}");
        Console.WriteLine($"Active employees: {activeEmployees.Count}");
        Console.WriteLine($"Inactive employees: {employees.Count - activeEmployees.Count}");
        Console.WriteLine($"Employees allocated today: {allocatedEmployeeIds.Count}");
        Console.WriteLine($"Bench employees: {activeEmployees.Count(e => !allocatedEmployeeIds.Contains(e.Id))}");
        Console.WriteLine($"Total projects: {projects.Count}");
        Console.WriteLine($"Active projects: {projects.Count(p => p.Status == ProjectStatus.Active)}");
        Console.WriteLine($"Total allocation records: {allocations.Count}");
    }

    private async Task DepartmentUtilizationAsync()
    {
        var employees = await employeeService.GetAllAsync();
        var allocations = await allocationRepository.GetAllAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);

        Console.WriteLine("\n--- Department Utilization ---");

        foreach (var group in employees
                     .Where(e => e.IsActive)
                     .GroupBy(e => e.Department))
        {
            var employeeIds = group.Select(e => e.Id).ToHashSet();

            var allocatedPercentage = allocations
                .Where(a => employeeIds.Contains(a.EmployeeId) &&
                            a.Duration.Start <= today &&
                            a.Duration.End >= today)
                .Sum(a => a.Percentage.Value);

            var capacity = group.Count() * 100m;
            var utilization = capacity == 0
                ? 0
                : allocatedPercentage / capacity * 100m;

            Console.WriteLine(
                $"{group.Key} | Employees: {group.Count()} | " +
                $"Allocated capacity: {allocatedPercentage}% | " +
                $"Utilization: {utilization:F2}%");
        }

        await Task.CompletedTask;
    }

    private static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Console.WriteLine("This field is required.");
        }
    }

    private static Guid ReadGuid(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            if (Guid.TryParse(Console.ReadLine(), out var value))
            {
                return value;
            }

            Console.WriteLine("Enter a valid GUID.");
        }
    }

    private static DateOnly ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            if (DateOnly.TryParseExact(
                    Console.ReadLine(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                return date;
            }

            Console.WriteLine("Enter a valid date in yyyy-MM-dd format.");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            if (decimal.TryParse(
                    Console.ReadLine(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var value))
            {
                return value;
            }

            Console.WriteLine("Enter a valid number.");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            if (int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            Console.WriteLine("Enter a valid integer.");
        }
    }

    private static TEnum ReadEnum<TEnum>(string label)
        where TEnum : struct, Enum
    {
        while (true)
        {
            Console.WriteLine($"{label}. Options: {string.Join(", ", Enum.GetNames<TEnum>())}");
            Console.Write("Enter value: ");

            if (Enum.TryParse<TEnum>(Console.ReadLine(), true, out var value) &&
                Enum.IsDefined(value))
            {
                return value;
            }

            Console.WriteLine("Invalid selection.");
        }
    }
}

