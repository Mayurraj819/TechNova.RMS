namespace TechNova.RMS.Domain.ValueObjects;

public sealed record DateRange
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    public DateRange(DateOnly start, DateOnly end)
    {
        if (end < start)
        {
            throw new ArgumentException(
                "End date cannot be before start date.");
        }

        Start = start;
        End = end;
    }

    public bool Overlaps(DateRange other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Start <= other.End &&
               other.Start <= End;
    }

    public bool Contains(DateRange other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Start <= other.Start &&
               End >= other.End;
    }
}