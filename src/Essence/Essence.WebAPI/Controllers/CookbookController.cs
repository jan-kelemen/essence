using Essence.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Essence.WebAPI.Controllers
{
    internal record struct Recipe(string Name, string Description);

    [ApiController]
    [Route("api/[controller]")]
    public class CookbookController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, Recipe> _recipes = new();

        [HttpPut("AddRecipe")]
        public async Task<Results<CreatedAtRoute<AddRecipeResponseDto>, Conflict>> AddRecipe([FromBody] AddRecipeRequestDto request)
        {
            if (!_recipes.TryAdd(request.Name, new Recipe(request.Name, request.Description)))
            {
                return TypedResults.Conflict();
            }

            return TypedResults.CreatedAtRoute(
                new AddRecipeResponseDto { Id = request.Name },
                nameof(GetRecipe), new { Id = request.Name });
        }

        [HttpGet("GetRecipe/{id}", Name = nameof(GetRecipe))]
        public async Task<Results<Ok, NotFound>> GetRecipe([FromRoute] string id)
        {
            if (!_recipes.TryGetValue(id, out _))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok();
        }
    }
}
