using System.ComponentModel.DataAnnotations;

namespace Essence.WebAPI.Models.Endpoints.Ingredient;

public readonly struct AddIngredientRequestDto
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; init; }

    public string? Summary { get; init; }

    public string? Description { get; init; }
}
