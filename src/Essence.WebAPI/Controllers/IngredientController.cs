using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Essence.Base.Validation;
using Essence.Domain.Services;
using Essence.WebAPI.Models;
using Essence.WebAPI.Models.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Essence.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientController(IIngredientService ingredientService)
    {
        _ingredientService = Ensure.That.IsNotNull(ingredientService);
    }

    [HttpGet("GetIngredient/{id}", Name = nameof(GetIngredient))]
    [ProducesResponseType(typeof(IngredientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetIngredient([FromRoute] string id)
    {
        var result = await _ingredientService.GetIngredient(new(id));

        return result.MapOrElse<IResult>(
            s => TypedResults.Ok(new IngredientDto(s.Id.Key, s.Name, s.Summary, s.Description)),
            e => e switch
            {
                GetIngredientError.NotFound => TypedResults.NotFound(),
                _ => TypedResults.StatusCode(StatusCodes.Status500InternalServerError)
            });
    }

    [HttpGet("QueryIngredients")]
    [ProducesResponseType(typeof(IEnumerable<IngredientHeaderDto>), StatusCodes.Status200OK)]
    public async Task<IResult> QueryIngredients([FromBody] QueryIngredientsRequestDto request)
    {
        var result = await _ingredientService.QueryIngredients(request.Prefix);

        return result.MapOrElse<IResult>(
            s => TypedResults.Ok(s.Select(i => new IngredientHeaderDto(i.Id.Key, i.Name))),
            e => e switch
            {
                _ => TypedResults.StatusCode(StatusCodes.Status500InternalServerError)
            });
    }

    [HttpPut("AddIngredient")]
    [ProducesResponseType(typeof(IngredientHeaderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> AddIngredient([FromBody] AddIngredientRequestDto request)
    {
        var result = await _ingredientService.AddIngredient(new(request.Name, request.Summary, request.Description));

        return result.MapOrElse<IResult>(
            s => TypedResults.CreatedAtRoute(new IngredientHeaderDto { Id = s.Key, Name = request.Name }, nameof(GetIngredient), new { Id = s.Key }),
            e => e switch
            {
                AddIngredientError.Conflict => TypedResults.Conflict(),
                _ => TypedResults.StatusCode(StatusCodes.Status500InternalServerError)
            });
    }

    [HttpPost("UpdateIngredient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IResult> UpdateIngredient([FromBody] UpdateIngredientRequestDto request)
    {
        var result = await _ingredientService.UpdateIngredient(new(new(request.Id), request.Name, request.Summary, request.Description));

        if (result.HasValue(out var error))
        {
            return error switch
            {
                UpdateIngredientError.NotFound => TypedResults.NotFound(),
                UpdateIngredientError.Conflict => TypedResults.Conflict(),
                _ => TypedResults.StatusCode(StatusCodes.Status500InternalServerError),
            };
        }

        return TypedResults.Ok();
    }

    [HttpDelete("DeleteIngredient/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteIngredient([FromRoute] string id)
    {
        var result = await _ingredientService.DeleteIngredient(new(id));

        if (result.HasValue(out var error))
        {
            return error switch
            {
                DeleteIngredientError.NotFound => TypedResults.NotFound(),
                _ => TypedResults.StatusCode(StatusCodes.Status500InternalServerError),
            };
        }

        return TypedResults.Ok();
    }
}
