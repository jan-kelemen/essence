namespace Essence.WebAPI.Models;

public readonly record struct IngredientHeaderDto(string Id, string Name);

public readonly record struct IngredientDto(string Id, string Name, string? Summary, string? Description);
