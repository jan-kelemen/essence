using System.ComponentModel.DataAnnotations;

namespace Essence.WebAPI.Models.Endpoints;

public class AddRecipeRequestDto
{
    [Required]
    public required string Name { get; init; }

    public string Description { get; init; } = string.Empty;
}
