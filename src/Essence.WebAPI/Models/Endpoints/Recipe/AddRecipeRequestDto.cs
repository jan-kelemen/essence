using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Essence.WebAPI.Models.Endpoints.Recipe;

public class AddRecipeRequestDto
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; init; }

    public string Description { get; init; } = string.Empty;

    [Required, MinLength(1)]
    public required IEnumerable<RecipeIngredientDto> Ingredients { get; init; }
}

public class AddRecipeErrorDto : ProblemDetails
{
    [JsonIgnore]
    public IEnumerable<string>? MissingIngredients
    {
        get => (IEnumerable<string>?)Extensions[nameof(MissingIngredients)];
        init => Extensions[nameof(MissingIngredients)] = value;
    }

    [JsonIgnore]
    public string? InvalidData
    {
        get => (string?)Extensions[nameof(InvalidData)];
        init => Extensions[nameof(InvalidData)] = value;
    }
}
