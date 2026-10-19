namespace TechNova.RMS.Infrastructure.Persistence;

public interface IJsonFileStore<T>
{
    Task<IReadOnlyList<T>> ReadAllAsync(
        CancellationToken cancellationToken = default);

    Task WriteAllAsync(
        IReadOnlyCollection<T> items,
        CancellationToken cancellationToken = default);
}