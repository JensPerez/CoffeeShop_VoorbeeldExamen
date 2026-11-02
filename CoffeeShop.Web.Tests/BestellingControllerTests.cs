using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using CoffeeShop.Web.Configuration;
using CoffeeShop.Web.Controllers;
using CoffeeShop.Web.Data;
using CoffeeShop.Web.Data.Repositories;
using CoffeeShop.Web.Models;
using CoffeeShop.Web.Repositories;
using CoffeeShop.Web.Services;
using CoffeeShop.Web.ViewModels;

namespace CoffeeShop.Web.Tests;

[TestFixture]
public class BestellingControllerTests
{
    private Mock<IUnitOfWork> _uow;
    private Mock<IBestellingRepo> _bestellingRepo;
    private BestellingController _controller;

    [OneTimeSetUp]
    public void OneTimeSetUp() => TypeAdapterConfig.GlobalSettings.Scan(typeof(MapperProfile).Assembly);

    [SetUp]
    public void SetUp()
    {
        _uow = new Mock<IUnitOfWork>();
        _bestellingRepo = new Mock<IBestellingRepo>();
        var koffieRepo = new Mock<IKoffieRepo>();
        var ingredientRepo = new Mock<IIngredientRepo>();

        _uow.Setup(x => x.BestellingRepo).Returns(_bestellingRepo.Object);
        _uow.Setup(x => x.KoffieRepo).Returns(koffieRepo.Object);
        _uow.Setup(x => x.IngredientRepo).Returns(ingredientRepo.Object);
        koffieRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Koffie>
        {
            new() { Id = 1, Naam = "Espresso", Prijs = 2m },
            new() { Id = 2, Naam = "Americano", Prijs = 3m }
        });
        ingredientRepo.Setup(x => x.GetIngredientenForKoffieAsync(It.IsAny<int>())).ReturnsAsync(new List<Ingredient>());
        koffieRepo.Setup(x => x.GetExistingIdsAsync()).ReturnsAsync(new[] { 1, 2 });

        _controller = new BestellingController(_uow.Object, new BestellingService(_uow.Object))
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
    }

    [TearDown]
    public void TearDown() => _controller.Dispose();

    [Test]
    public async Task Index_BerekentTotaalInclusiefLeverkosten()
    {
        _bestellingRepo.Setup(x => x.GetAllWithDetailsAsync()).ReturnsAsync(new List<Bestelling>
        {
            new() { Id = 1, Leverkosten = 5m, KoffiesInBestelling = { new BesteldeKoffie { Aantal = 2, Koffie = new Koffie { Naam = "Espresso", Prijs = 2m } } } }
        });

        var result = await _controller.Index() as ViewResult;
        var vm = result?.Model as List<BestellingOverzichtVm>;

        Assert.That(vm![0].TotaalPrijs, Is.EqualTo(9m));
    }

    [Test]
    public async Task Create_Get_SelecteertMeegegevenKoffie()
    {
        var result = await _controller.Create(koffieId: 2) as ViewResult;
        var vm = result?.Model as BestellingFormVm;

        Assert.That(vm!.Regels, Has.Count.EqualTo(2));
        Assert.That(vm.Regels.Single(x => x.KoffieId == 2).Aantal, Is.EqualTo(1));
        Assert.That(vm.Regels.Single(x => x.KoffieId == 1).Aantal, Is.EqualTo(0));
    }

    [Test]
    public async Task Create_Post_ZonderKoffies_ToontFormulierOpnieuw()
    {
        var vm = new BestellingFormVm { KlantVoorNaam = "Jan", KlantAchterNaam = "Peeters", Regels = { new() { KoffieId = 1, Aantal = 0 } } };

        var result = await _controller.Create(vm);

        Assert.That(result, Is.InstanceOf<ViewResult>());
        Assert.That(_controller.ModelState.IsValid, Is.False);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task Create_Post_Geldig_SlaatOpEnRedirect()
    {
        var vm = new BestellingFormVm { KlantVoorNaam = "Jan", KlantAchterNaam = "Peeters", Regels = { new() { KoffieId = 1, Aantal = 2 } } };

        var result = await _controller.Create(vm);

        Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        _bestellingRepo.Verify(x => x.Add(It.Is<Bestelling>(b => b.Leverkosten == 5m && b.KoffiesInBestelling.Count == 1)), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
