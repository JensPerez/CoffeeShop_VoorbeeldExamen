using Moq;
using CoffeeShop.Web.Data;
using CoffeeShop.Web.Models;
using CoffeeShop.Web.Repositories;
using CoffeeShop.Web.Services;
using CoffeeShop.Web.ViewModels;

namespace CoffeeShop.Web.Tests;

[TestFixture]
public class BestellingServiceCrudTests
{
    private Mock<IUnitOfWork> _uow;
    private Mock<IBestellingRepo> _bestellingRepo;
    private Mock<IIngredientRepo> _ingredientRepo;
    private BestellingService _service;

    private readonly List<Koffie> _koffies = new()
    {
        new Koffie { Id = 1, Naam = "Espresso", Prijs = 2m },
        new Koffie { Id = 2, Naam = "Mocha", Prijs = 20m }
    };

    [SetUp]
    public void SetUp()
    {
        _uow = new Mock<IUnitOfWork>();
        _bestellingRepo = new Mock<IBestellingRepo>();
        _ingredientRepo = new Mock<IIngredientRepo>();
        var koffieRepo = new Mock<IKoffieRepo>();

        _uow.Setup(x => x.BestellingRepo).Returns(_bestellingRepo.Object);
        _uow.Setup(x => x.KoffieRepo).Returns(koffieRepo.Object);
        _uow.Setup(x => x.IngredientRepo).Returns(_ingredientRepo.Object);
        koffieRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(_koffies);
        koffieRepo.Setup(x => x.GetExistingIdsAsync()).ReturnsAsync(new[] { 1, 2 });
        _ingredientRepo.Setup(x => x.GetIngredientenForKoffieAsync(It.IsAny<int>())).ReturnsAsync(new List<Ingredient>());

        _service = new BestellingService(_uow.Object);
    }

    private static BestellingFormVm MaakVm(params (int koffieId, int aantal)[] regels) => new()
    {
        KlantVoorNaam = "Jan",
        KlantAchterNaam = "Peeters",
        Regels = regels.Select(x => new BestelRegelVm { KoffieId = x.koffieId, Aantal = x.aantal }).ToList()
    };

    [Test]
    public async Task ValideerRegelsAsync_GeenKoffies_GeeftFoutmelding()
    {
        Assert.That(await _service.ValideerRegelsAsync(MaakVm(regels: (1, 0)).Regels), Does.Contain("minstens één"));
    }

    [TestCase(99, 1)]
    [TestCase(1, -1)]
    public async Task ValideerRegelsAsync_OngeldigeKoffieOfAantal_GeeftFoutmelding(int koffieId, int aantal)
    {
        Assert.That(await _service.ValideerRegelsAsync(MaakVm(regels: (koffieId, aantal)).Regels), Does.Contain("Ongeldige"));
    }

    [Test]
    public async Task ValideerRegelsAsync_Geldig_GeeftNull()
    {
        Assert.That(await _service.ValideerRegelsAsync(MaakVm(regels: (1, 2)).Regels), Is.Null);
    }

    [Test]
    public async Task MaakBestellingAsync_VoegtRegelsSamenEnBerekentLeverkosten()
    {
        BestellingCreatedVm result = await _service.MaakBestellingAsync(MaakVm(regels: new[] { (1, 2), (1, 1), (2, 0) }));

        Assert.That(result.KlantNaam, Is.EqualTo("Jan Peeters"));
        Assert.That(result.Subtotaal, Is.EqualTo(6m));
        Assert.That(result.Leverkosten, Is.EqualTo(5m));
        _bestellingRepo.Verify(x => x.Add(It.Is<Bestelling>(b => b.KoffiesInBestelling.Count == 1 && b.KoffiesInBestelling[0].Aantal == 3 && b.Leverkosten == 5m)), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void MaakBestellingAsync_OnvoldoendeVoorraad_GooitExceptionEnSlaatNietOp()
    {
        _ingredientRepo.Setup(x => x.GetIngredientenForKoffieAsync(1))
            .ReturnsAsync(new List<Ingredient> { new() { Naam = "Espressobonen", AantalInStock = 1 } });

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.MaakBestellingAsync(MaakVm(regels: (1, 5))));
        _uow.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

}
