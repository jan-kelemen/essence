using System.ComponentModel.DataAnnotations;

namespace Essence.WebAPI.Models.Endpoints.Ingredient;

public readonly struct QueryIngredientsRequestDto
{
    [Required]
    public required string Prefix { get; init; }
}
