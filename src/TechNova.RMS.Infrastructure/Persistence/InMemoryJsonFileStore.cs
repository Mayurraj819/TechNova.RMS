using System.Collections.Concurrent;

namespace TechNova.RMS.Infrastructure.Persistence;

/// <summary>
/// Lightweight in-memory implementation of IJsonFileStore<T> for tests and local scenarios.
/// Not intended for production concurrency across processes.
/// </summary>
public sealed class InMemoryJsonFileStore<T> : IJsonFileStore<T>
    where T : class
{
    private readonly List<T> _items = new();
    private readonly SemaphoreSlim _gate = new(1, 1);

    public InMemoryJsonFileStore()
    {
    }

    public async Task<IReadOnlyList<T>> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);

        try
        {
            // Return a shallow copy to protect callers from modifying internal list
            return _items.ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task WriteAllAsync(IReadOnlyCollection<T> items, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);

        try
        {
            _items.Clear();
            _items.AddRange(items);
        }
        finally
        {
            _gate.Release();
        }
    }
}
