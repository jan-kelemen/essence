using System.Collections.Generic;
using System.Threading.Tasks;
using Essence.Domain.Model;
using Essence.Domain.Repositories;
using Essence.Domain.Services;

namespace Essence.Persistence.Postgre.Test.Integration;

internal class DataGenerator
{
    public static async Task<List<IngredientHeader>> GenerateIngredients(
        IIngredientRepository ingredientRepository,
        string namePrefix,
        int count)
    {
        var rv = new List<IngredientHeader>();

        for (var i = 0; i < count; i++)
        {
            var name = $"{namePrefix}_{i}";

            var ingredient = new NewIngredient(name, null, null);
            var addResult = await ingredientRepository.AddIngredient(ingredient);

            rv.Add(new(addResult.Expect(), name));
        }

        return rv;
    }
}
