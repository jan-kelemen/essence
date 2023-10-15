using Essence.Domain.Vocabulary;

namespace Essence.Domain.Repositories;
public record RepositoryError
{
    public record Conflict(string? Property) : RepositoryError;

    public record NotFound(Identifier Id) : RepositoryError;

    public record UnresolvedEntites(string? EntityType) : RepositoryError;

    public record InvalidData(string? EntityType, string? Property) : RepositoryError;

    private RepositoryError() { }
}
