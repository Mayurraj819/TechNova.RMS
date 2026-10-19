namespace TechNova.RMS.Domain.ValueObjects;

public sealed record AllocationPercentage
{
    public int Value { get; }

    public AllocationPercentage(int value)
    {
        if (value is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Allocation must be between 1 and 100.");
        }

        Value = value;
    }
}