using TechNova.RMS.Application.Interfaces;

namespace TechNova.RMS.Infrastructure.Repositories;

public abstract class JsonRepositoryBase<TDto, TEntity> : IRepository<TEntity>
    where TDto : class
    where TEntity : class
{
    protected readonly IJsonFileStore<TDto> Store;
    private readonly SemaphoreSlim _cacheGate = new(1, 1);
    private Dictionary<Guid, TDto>? _index;

    protected JsonRepositoryBase(IJsonFileStore<TDto> store)
    {
        Store = store ?? throw new ArgumentNullException(nameof(store));
    }

    protected abstract TDto ToDto(TEntity entity);
    protected abstract TEntity ToDomain(TDto dto);
    protected abstract Guid GetId(TDto dto);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var records = await Store.ReadAllAsync(cancellationToken);
        // rebuild index to reflect latest persisted state
        await RebuildIndexAsync(records, cancellationToken);
        return records.Select(ToDomain).ToList();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await EnsureIndexAsync(cancellationToken);

        if (_index is null)
            return null;

        return _index.TryGetValue(id, out var dto) ? ToDomain(dto) : null;
    }

    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var records = (await Store.ReadAllAsync(cancellationToken)).ToList();

        var dto = ToDto(entity);
        if (records.Any(x => GetId(x) == GetId(dto)))
            throw new InvalidOperationException("Entity already exists.");

        records.Add(dto);
        await Store.WriteAllAsync(records, cancellationToken);

        // update index
        await RebuildIndexAsync(records, cancellationToken);
    }

    public virtual async Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var records = (await Store.ReadAllAsync(cancellationToken)).ToList();
        var dto = ToDto(entity);
        var id = GetId(dto);
        var index = records.FindIndex(x => GetId(x) == id);

        if (index < 0)
            throw new KeyNotFoundException("Entity not found.");

        records[index] = dto;
        await Store.WriteAllAsync(records, cancellationToken);

        // update index
        await RebuildIndexAsync(records, cancellationToken);
    }

    public virtual async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var records = (await Store.ReadAllAsync(cancellationToken)).ToList();

        var index = records.FindIndex(x => GetId(x) == id);

        if (index < 0)
            throw new InvalidOperationException("Record not found.");

        records.RemoveAt(index);
        await Store.WriteAllAsync(records, cancellationToken);

        // update index
        await RebuildIndexAsync(records, cancellationToken);
    }

    private async Task EnsureIndexAsync(CancellationToken cancellationToken)
    {
        if (_index is not null)
            return;

        await _cacheGate.WaitAsync(cancellationToken);
        try
        {
            if (_index is not null)
                return;

            var records = await Store.ReadAllAsync(cancellationToken);
            _index = records.ToDictionary(GetId);
        }
        finally
        {
            _cacheGate.Release();
        }
    }

    private async Task RebuildIndexAsync(IReadOnlyList<TDto> records, CancellationToken cancellationToken)
    {
        await _cacheGate.WaitAsync(cancellationToken);
        try
        {
            _index = records.ToDictionary(GetId);
        }
        finally
        {
            _cacheGate.Release();
        }
    }
}
