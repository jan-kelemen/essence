using Essence.Domain.Vocabulary;

namespace Essence.Domain.Repositories;
public record RepositoryError
{
    public record Conflict(string? Property) : RepositoryError;

    public record NotFound(Identifier Id) : RepositoryError;

    private RepositoryError() { }
}
