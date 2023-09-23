using Essence.Base.Validation;

namespace Essence.Persistence.Postgre.Configuration;

public sealed record class PostgreConnectionOptions
{
    private string _connectionString = string.Empty;

    public const string PostgreConnection = $"{nameof(Essence)}:{nameof(PostgreConnection)}";

    public string ConnectionString
    { 
        get => _connectionString;
        set => _connectionString = Ensure.That.IsNotNullOrEmpty(value);
    }
}