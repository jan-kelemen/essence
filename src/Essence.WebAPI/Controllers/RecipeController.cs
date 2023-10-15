using Essence.Base.Validation;
using Essence.Domain.Model;
using Essence.Domain.Services;
using Essence.WebAPI.Models;
using Essence.WebAPI.Models.Endpoints.Recipe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Essence.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = Ensure.That.IsNotNull(recipeService);
    }

    [HttpPut("AddRecipe")]
    [ProducesResponseType(typeof(RecipeHeaderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(RecipeHeaderDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(AddRecipeErrorDto), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> AddRecipe([FromBody] AddRecipeRequestDto request)
    {
        var result = await _recipeService.AddRecipe(
            new(
                request.Name,
                request.Description,
                request.Ingredients.Select(i => new RecipeIngredient(new(new(i.Ingredient.Id), i.Ingredient.Name), i.Amount))));

        return result.MapOrElse<IResult>(
            s => TypedResults.CreatedAtRoute(
                new RecipeHeaderDto { Id = s.Key, Name = request.Name },
                nameof(GetRecipe),
                new { Id = s.Key }),
            e => e switch
            {
                AddRecipeError.Conflict c => c.ConflictingRecipe.HasValue ? TypedResults.Conflict(new RecipeHeaderDto
                {
                    Id = c.ConflictingRecipe.Value.Id.Key,
                    Name = c.ConflictingRecipe.Value.Name,
                }) : TypedResults.Conflict(),
                AddRecipeError.UnknownIngredients ui => TypedResults.UnprocessableEntity(new AddRecipeErrorDto
                {
                    Title = "Request contains missing ingredients",
                    Status = 422,                    
                    MissingIngredients = ui.Ingredients.Select(i => i.Ingredient.Id.Key)
                }),
                AddRecipeError.InvalidData id => TypedResults.UnprocessableEntity(new AddRecipeErrorDto
                {
                    Title = "Request contains invalid data",
                    Status = 422,
                    InvalidData = $"{id.EntityName}/{id.PropertyName}"
                }),
                _ => throw new NotImplementedException()
            });
    }

    [HttpGet("GetRecipe/{id}", Name = nameof(GetRecipe))]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetRecipe([FromRoute] string id)
    {
        var result = await _recipeService.GetRecipe(new(id));

        return result.MapOrElse<IResult>(
            s => TypedResults.Ok(
                new RecipeDto(
                    s.Id.Key,
                    s.Name,
                    s.Description,
                    s.Ingredients.Select(i => new RecipeIngredientDto(
                        new IngredientHeaderDto(i.Ingredient.Id.Key, i.Ingredient.Name), i.Amount)))),
            e => TypedResults.NotFound());
    }
}
