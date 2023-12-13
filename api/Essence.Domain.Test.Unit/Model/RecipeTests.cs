using System;
using System.Collections.Immutable;
using System.Linq;
using Essence.Domain.Model;
using Xunit;

namespace Essence.Domain.Test.Unit.Model;
public class RecipeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Name_IsNeverEmpty(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new RecipeHeader(new("ID"), name!));
    }

    [Fact]
    public void Description_IsNeverNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Recipe(new RecipeHeader(new("ID"), "Name"))
        {
            Description = null!,
        });

        var recepeWithUnspecifiedDescription = new Recipe(new RecipeHeader(new("ID"), "Name"));
        Assert.NotNull(recepeWithUnspecifiedDescription.Description);
    }

    [Fact]
    public void Amount_OfIngredient_FitsIntoInteger()
    {
        Assert.Throws<OverflowException>(() => new Recipe(new RecipeHeader(new("ID"), "Name"))
        {
            Ingredients = new[]
            {
                new RecipeIngredient(new IngredientHeader(new("ID"), "Salt"), uint.MaxValue)
            }
        });
    }

    [Fact]
    public void IdenticalIngredientIsCombinedIntoSingleEntry()
    {
        var salt = new IngredientHeader(new("SaltID"), "Salt");
        var pepper = new IngredientHeader(new("PepperID"), "Pepper");

        var recipe = new Recipe(new RecipeHeader(new("ID"), "Salt & Pepper"))
        {
            Ingredients = new[]
            {
                new RecipeIngredient(salt, 5),
                new RecipeIngredient(pepper, 10),
                new RecipeIngredient(salt, 10)
            }
        };

        var recipeIngredients = recipe.Ingredients.ToImmutableArray();

        Assert.Equal(2, recipeIngredients.Length);
        Assert.Equal((uint)15, recipeIngredients.First(x => x.Ingredient == salt).Amount);
        Assert.Equal((uint)10, recipeIngredients.First(x => x.Ingredient == pepper).Amount);
    }
}
