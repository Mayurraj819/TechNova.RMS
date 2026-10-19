namespace TechNova.RMS.Domain.ValueObjects;

public sealed record Skill
{
    public string Name { get; }
    public int Proficiency { get; }

    public Skill(string name, int proficiency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (proficiency is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(proficiency),
                "Proficiency must be between 1 and 5.");
        }

        Name = name.Trim();
        Proficiency = proficiency;
    }
}