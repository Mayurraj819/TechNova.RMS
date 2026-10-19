namespace TechNova.RMS.Infrastructure.Persistence.Models;

public sealed class AllocationDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ProjectId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Percentage { get; set; }
}