using Essence.Base.Validation;
using Essence.Base.Vocabulary;
using Essence.Domain.Model;
using Essence.Domain.Services;
using Essence.Domain.Vocabulary;
using Essence.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
        public async Task<Results<CreatedAtRoute<AddRecipeResponseDto>, Conflict>> AddRecipe([FromBody] AddRecipeRequestDto request)
        {
            var result = await _cookbookService.AddRecipe(new(request.Name, request.Description));

            if (result.IsSuccess)
            {
                return TypedResults.CreatedAtRoute(
                    new AddRecipeResponseDto { Id = result.Expect.Key }, nameof(GetRecipe), new { Id = result.Expect.Key });
            }

            return result.ExpectError switch
            {
                AddRecipeError.Conflict => TypedResults.Conflict(),
                _ => throw new UnreachableException()
            };
        }

        [HttpGet("GetRecipe/{id}", Name = nameof(GetRecipe))]
        public async Task<Results<Ok, NotFound>> GetRecipe([FromRoute] string id)
        {
            var result = await _cookbookService.GetRecipe(new(id));

            if (result.IsSuccess)
            {
                return TypedResults.Ok();
            }

            return result.ExpectError switch
            {
                GetRecipeError.NotFound => TypedResults.NotFound(),
                _ => throw new UnreachableException()
            };
        }
    }
}
