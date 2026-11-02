using CoffeeShop.Web.Services;
using CoffeeShop.Web.ViewModels;

namespace CoffeeShop.Web.Tests;

[TestFixture]
public class VeganServiceTests
{
    private readonly VeganService _service = new();

    [Test]
    public void BepaalVegan_AlleIngredientenVegan_GeeftTrue()
    {
        var ingredienten = new List<IngredientVm> { new() { IsVegan = true }, new() { IsVegan = true } };

        Assert.That(_service.BepaalVegan(ingredienten), Is.True);
    }

    [Test]
    public void BepaalVegan_EenIngredientNietVegan_GeeftFalse()
    {
        var ingredienten = new List<IngredientVm> { new() { IsVegan = true }, new() { IsVegan = false } };

        Assert.That(_service.BepaalVegan(ingredienten), Is.False);
    }
}
