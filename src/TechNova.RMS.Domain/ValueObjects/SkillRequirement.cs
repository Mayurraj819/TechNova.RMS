namespace TechNova.RMS.Domain.ValueObjects;

public sealed record SkillRequirement
{
    public string SkillName { get; }
    public int MinimumProficiency { get; }

    public SkillRequirement(
        string skillName,
        int minimumProficiency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(skillName);

        if (minimumProficiency is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumProficiency),
                "Minimum proficiency must be between 1 and 5.");
        }

        SkillName = skillName.Trim();
        MinimumProficiency = minimumProficiency;
    }
}