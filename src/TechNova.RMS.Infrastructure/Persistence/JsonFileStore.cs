using System.Text.Json;

namespace TechNova.RMS.Infrastructure.Persistence;

public sealed class JsonFileStore<T> : IJsonFileStore<T>
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonFileStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = Path.GetFullPath(filePath);
    }

    public async Task<IReadOnlyList<T>> ReadAllAsync(
        CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);

        try
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            await using var stream = File.OpenRead(_filePath);

            return await JsonSerializer.DeserializeAsync<List<T>>(
                stream, _options, cancellationToken) ?? new List<T>();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task WriteAllAsync(
        IReadOnlyCollection<T> items,
        CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);

        try
        {
            var directory = Path.GetDirectoryName(_filePath)
                ?? throw new InvalidOperationException(
                    "The data directory could not be determined.");

            Directory.CreateDirectory(directory);

            var temporaryPath = _filePath + ".tmp";

            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                useAsync: true))
            {
                await JsonSerializer.SerializeAsync(
                    stream, items, _options, cancellationToken);

                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, _filePath, overwrite: true);
        }
        finally
        {
            _gate.Release();
        }
    }
}