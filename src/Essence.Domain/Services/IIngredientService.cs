using System.Collections.Generic;
using System.Threading.Tasks;
using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Vocabulary;

namespace Essence.Domain.Services;

public readonly record struct NewIngredient(string Name, string? Summary, string? Description)
{
    public string Name { get; } = Ensure.That.IsNotNullOrWhitespace(Name);
}

public readonly record struct UpdatedIngredient(Identifier Id, string Name, string? Summary, string? Description)
{
    public Identifier Id { get; } = Ensure.That.IsNotNull(Id);

    public string Name { get; } = Ensure.That.IsNotNullOrWhitespace(Name);
}

public record GetIngredientError
{
    public record NotFound(Identifier Id) : GetIngredientError;

    private GetIngredientError() { }
}

public record QueryIngredientsError
{
    private QueryIngredientsError() { }
}

public record AddIngredientError
{
    public record Conflict(Identifier? ConflictingIngredient) : AddIngredientError;

    private AddIngredientError() { }
}

public record UpdateIngredientError
{
    public record Conflict(Identifier? ConflictingIngredient) : UpdateIngredientError;

    public record NotFound(Identifier Id) : UpdateIngredientError;

    private UpdateIngredientError() { }
}

public record DeleteIngredientError
{
    public record NotFound(Identifier Id) : DeleteIngredientError;

    private DeleteIngredientError() { }
}

public interface IIngredientService
{
    public Task<Result<Ingredient, GetIngredientError>> GetIngredient(Identifier id);

    public Task<Result<IEnumerable<IngredientHeader>, QueryIngredientsError>> QueryIngredients(string filter);

    public Task<Result<Identifier, AddIngredientError>> AddIngredient(NewIngredient ingredient);

    public Task<Option<UpdateIngredientError>> UpdateIngredient(UpdatedIngredient ingredient);

    public Task<Option<DeleteIngredientError>> DeleteIngredient(Identifier id);
}
