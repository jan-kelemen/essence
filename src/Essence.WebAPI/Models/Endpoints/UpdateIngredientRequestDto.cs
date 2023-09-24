using System.ComponentModel.DataAnnotations;

namespace Essence.WebAPI.Models.Endpoints;

public readonly struct UpdateIngredientRequestDto
{
    [Required]
    public required string Id { get; init; }

    [Required]
    public required string Name { get; init; }

    public string? Summary { get; init; }

    public string? Description { get; init; }
}
