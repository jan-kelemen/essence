using Essence.Base.Validation;
using Essence.Domain.Services;
using Essence.WebAPI.Models;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(typeof(AddRecipeResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IResult> AddRecipe([FromBody] AddRecipeRequestDto request)
        {
            var result = await _cookbookService.AddRecipe(new(request.Name, request.Description));

            return result.MapOrElse<IResult>(
                s => TypedResults.CreatedAtRoute(new AddRecipeResponseDto { Id = s.Key }, nameof(GetRecipe), new { Id = s.Key }),
                e => TypedResults.Conflict());
        }

        [HttpGet("GetRecipe/{id}", Name = nameof(GetRecipe))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetRecipe([FromRoute] string id)
        {
            var result = await _cookbookService.GetRecipe(new(id));

            return result.MapOrElse<IResult>(
                s => TypedResults.Ok(),
                e => TypedResults.NotFound());
        }
    }
}
