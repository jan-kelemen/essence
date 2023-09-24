using Essence.Persistence.Postgre.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Essence.Persistence.Postgre.Test.Integration;

public class PostgreFixture : IDisposable
{
    private PostgreConnectionProvider? _connectionProvider;

    public PostgreFixture()
    {
        var configurationRoot = new ConfigurationBuilder()
            .AddUserSecrets<PostgreFixture>()
            .Build();

        var options = configurationRoot
            .GetSection(PostgreConnectionOptions.PostgreConnection)
            .Get<PostgreConnectionOptions>()!;

        _connectionProvider = new PostgreConnectionProvider(
            Options.Create(options));
    }

    internal IPostgreConnectionProvider ConnectionProvider => _connectionProvider!;

    protected virtual void Dispose(bool disposing)
    {
        if (_connectionProvider is not null)
        {
            if (disposing)
            {
                _connectionProvider.Dispose();
            }
            _connectionProvider = null;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
