using TechNova.RMS.Domain.Enums;
using TechNova.RMS.Domain.ValueObjects;

namespace TechNova.RMS.Domain.Entities;

public sealed class Project
{
    private readonly List<SkillRequirement> _requiredSkills = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Client { get; private set; }
    public DateRange Duration { get; private set; }
    public ProjectStatus Status { get; private set; }

    public IReadOnlyList<SkillRequirement> RequiredSkills =>
        _requiredSkills.AsReadOnly();

    public Project(
        string name,
        string client,
        DateRange duration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(client);
        ArgumentNullException.ThrowIfNull(duration);

        Id = Guid.NewGuid();
        Name = name.Trim();
        Client = client.Trim();
        Duration = duration;
        Status = ProjectStatus.Draft;
    }
    public static Project Reconstitute(
    Guid id,
    string name,
    string client,
    DateRange duration,
    ProjectStatus status,
    IEnumerable<SkillRequirement> requiredSkills)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Project ID is required.");

        ArgumentNullException.ThrowIfNull(requiredSkills);

        var project = new Project(name, client, duration)
        {
            Id = id,
            Status = status
        };

        foreach (var skill in requiredSkills)
            project.AddOrUpdateSkillRequirement(skill);

        return project;
    }
    public void UpdateDetails(
        string name,
        string client,
        DateRange duration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(client);
        ArgumentNullException.ThrowIfNull(duration);

        Name = name.Trim();
        Client = client.Trim();
        Duration = duration;
    }

    public void AddOrUpdateSkillRequirement(
        SkillRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(requirement);

        var existing = _requiredSkills.FindIndex(
            skill => string.Equals(
                skill.SkillName,
                requirement.SkillName,
                StringComparison.OrdinalIgnoreCase));

        if (existing >= 0)
        {
            _requiredSkills[existing] = requirement;
            return;
        }

        _requiredSkills.Add(requirement);
    }

    public void ChangeStatus(ProjectStatus status)
    {
        Status = status;
    }
}