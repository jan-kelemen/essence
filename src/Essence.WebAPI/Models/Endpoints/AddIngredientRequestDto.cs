using System.ComponentModel.DataAnnotations;

namespace Essence.WebAPI.Models.Endpoints;

public readonly struct AddIngredientRequestDto
{
    [Required]
    public required string Name { get; init; }

    public string? Summary { get; init; }

    public string? Description { get; init; }
}
