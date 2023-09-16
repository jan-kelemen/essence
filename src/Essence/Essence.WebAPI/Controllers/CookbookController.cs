using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Essence.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CookbookController : ControllerBase
    {
        private readonly ICookbookService _cookbookService;

        public CookbookController(ICookbookService cookbookService)
        {
            _cookbookService = Ensure.That.IsNotNull(cookbookService);
        }

        [HttpPut("AddRecipe")]
        public async Task<Results<CreatedAtRoute<AddRecipeResponseDto>, Conflict, StatusCodeHttpResult>> AddRecipe([FromBody] AddRecipeRequestDto request)
        {
            var result = await _cookbookService.AddRecipe(new(request.Name, request.Description));

            return result switch
            {
                Result<Identifier, AddRecipeError>.Success succ =>
                  TypedResults.CreatedAtRoute(new AddRecipeResponseDto { Id = succ.Value.Key }, nameof(GetRecipe), new { Id = succ.Value.Key }),

                Result<Identifier, AddRecipeError>.Error err => err.Value switch
                {
                    AddRecipeError.Conflict => TypedResults.Conflict(),
                    _ => TypedResults.StatusCode(500)
                },

                _ => TypedResults.StatusCode(500)
            };

        }

        [HttpGet("GetRecipe/{id}", Name = nameof(GetRecipe))]
        public async Task<Results<Ok, NotFound, StatusCodeHttpResult>> GetRecipe([FromRoute] string id)
        {
            var result = await _cookbookService.GetRecipe(new(id));

            return result switch
            {
                Result<Recipe, GetRecipeError>.Success => TypedResults.Ok(),
                Result<Recipe, GetRecipeError>.Error err => err.Value switch
                {
                    GetRecipeError.NotFound => TypedResults.NotFound(),
                    _ => TypedResults.StatusCode(500)
                },
                _ => TypedResults.StatusCode(500)
            };
        }
    }
}
