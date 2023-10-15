using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Essence.WebAPI.Models;

public readonly record struct RecipeHeaderDto(
    [Required(AllowEmptyStrings = false)] string Id,
    [Required(AllowEmptyStrings = false)] string Name);

public readonly record struct RecipeIngredientDto(
    [Required] IngredientHeaderDto Ingredient,
    [Required, Range(0, int.MaxValue)] uint Amount);

public readonly record struct RecipeDto(
    [Required(AllowEmptyStrings = false)] string Id,
    [Required(AllowEmptyStrings = false)] string Name,
    string Description,
    [Required, MinLength(1)] IEnumerable<RecipeIngredientDto> Ingredients);
