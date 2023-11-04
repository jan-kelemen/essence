using Essence.Persistence.Postgre.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Essence.Persistence.Postgre;

public interface IPostgreConnectionProvider
{
    ValueTask<NpgsqlConnection> OpenConnection();
}

internal sealed class PostgreConnectionProvider : IPostgreConnectionProvider, IDisposable
{
    private readonly PostgreConnectionOptions _options;
    private NpgsqlDataSource? _dataSource;

    public PostgreConnectionProvider(IOptions<PostgreConnectionOptions> options)
    {
        _options = options.Value;

        var builder = new NpgsqlDataSourceBuilder(_options.ConnectionString);
        _dataSource = builder.Build();
    }

    public ValueTask<NpgsqlConnection> OpenConnection()
        => _dataSource!.OpenConnectionAsync();

    private void Dispose(bool disposing)
    {
        if (_dataSource is not null)
        {
            if (disposing)
            {
                _dataSource?.Dispose();
            }
            _dataSource = null;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}