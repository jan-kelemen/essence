using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;
using Essence.WebAPI.Models;
using Essence.WebAPI.Models.Endpoints.Ingredient;
using Xunit;

namespace Essence.Persistence.Postgre.Test.Integration;

internal class DataGenerator
{
    public static async Task<List<IngredientHeaderDto>> GenerateIngredients(
        HttpClient client,
        string namePrefix,
        int count)
    {
        var rv = new List<IngredientHeaderDto>();

        for (var i = 0; i < count; i++)
        {
            var name = $"{namePrefix}_{i}";

            var request = new AddIngredientRequestDto
            {
                Name = name,
                Description = null,
                Summary = null
            };

            var addResponse = await client.PutAsJsonAsync("api/Ingredient/AddIngredient", request);
            Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);

            var header = await addResponse.Content.ReadFromJsonAsync<IngredientHeaderDto>();

            rv.Add(header);
        }

        return rv;
    }
}
