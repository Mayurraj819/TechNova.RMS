using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Domain.Entities;

public sealed class Employee
{
    private readonly List<Skill> _skills = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Designation { get; private set; }
    public string Department { get; private set; }
    public EmploymentType EmploymentType { get; private set; }
    public decimal CostRate { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyList<Skill> Skills => _skills.AsReadOnly();

    public Employee(
        string name,
        string email,
        string designation,
        string department,
        EmploymentType employmentType,
        decimal costRate)
    {
        Id = Guid.NewGuid();

        UpdateDetails(
            name,
            email,
            designation,
            department,
            employmentType,
            costRate);

        IsActive = true;
    }
    public static Employee Reconstitute(
    Guid id,
    string name,
    string email,
    string designation,
    string department,
    EmploymentType employmentType,
    decimal costRate,
    bool isActive,
    IEnumerable<Skill> skills)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Employee ID is required.");

        ArgumentNullException.ThrowIfNull(skills);

        var employee = new Employee(
            name, email, designation, department,
            employmentType, costRate)
        {
            Id = id,
            IsActive = isActive
        };

        foreach (var skill in skills)
            employee.AddOrUpdateSkill(skill);

        return employee;
    }
    public void UpdateDetails(
        string name,
        string email,
        string designation,
        string department,
        EmploymentType employmentType,
        decimal costRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(designation);
        ArgumentException.ThrowIfNullOrWhiteSpace(department);

        if (!email.Contains('@') ||
            email.StartsWith('@') ||
            email.EndsWith('@'))
        {
            throw new ArgumentException(
                "Email address is invalid.",
                nameof(email));
        }

        if (costRate < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(costRate),
                "Cost rate cannot be negative.");
        }

        Name = name.Trim();
        Email = email.Trim();
        Designation = designation.Trim();
        Department = department.Trim();
        EmploymentType = employmentType;
        CostRate = costRate;
    }

    public void AddOrUpdateSkill(Skill skill)
    {
        ArgumentNullException.ThrowIfNull(skill);

        var existing = _skills.FindIndex(
            s => string.Equals(
                s.Name,
                skill.Name,
                StringComparison.OrdinalIgnoreCase));

        if (existing >= 0)
        {
            _skills[existing] = skill;
            return;
        }

        _skills.Add(skill);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}