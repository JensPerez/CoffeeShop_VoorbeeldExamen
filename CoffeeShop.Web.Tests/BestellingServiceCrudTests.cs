using Moq;
using CoffeeShop.Web.Data;
using CoffeeShop.Web.Data.Repositories;
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
    private Mock<IGenericRepo<BesteldeKoffie>> _besteldeKoffiesRepo;
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
        _besteldeKoffiesRepo = new Mock<IGenericRepo<BesteldeKoffie>>();
        var koffieRepo = new Mock<IKoffieRepo>();

        _uow.Setup(x => x.BestellingRepo).Returns(_bestellingRepo.Object);
        _uow.Setup(x => x.KoffieRepo).Returns(koffieRepo.Object);
        _uow.Setup(x => x.IngredientRepo).Returns(_ingredientRepo.Object);
        _uow.Setup(x => x.BesteldeKoffiesRepo).Returns(_besteldeKoffiesRepo.Object);
        koffieRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(_koffies);
        koffieRepo.Setup(x => x.GetExistingIdsAsync()).ReturnsAsync(new[] { 1, 2 });
        _ingredientRepo.Setup(x => x.GetIngredientenForKoffieAsync(It.IsAny<int>())).ReturnsAsync(new List<Ingredient>());

        _service = new BestellingService(_uow.Object);
    }

    private static BestellingFormVm MaakVm(string status = "In behandeling", params (int koffieId, int aantal)[] regels) => new()
    {
        KlantVoorNaam = "Jan",
        KlantAchterNaam = "Peeters",
        Status = status,
        Regels = regels.Select(x => new BestelRegelVm { KoffieId = x.koffieId, Aantal = x.aantal }).ToList()
    };

    private Bestelling MaakBestelling() => new()
    {
        Id = 7,
        KoffiesInBestelling =
        {
            new BesteldeKoffie { KoffieId = 1, Koffie = _koffies[0], Aantal = 1 },
            new BesteldeKoffie { KoffieId = 2, Koffie = _koffies[1], Aantal = 1 }
        }
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

    [Test]
    public async Task WerkBestellingBijAsync_Onbestaand_GeeftFalse()
    {
        _bestellingRepo.Setup(x => x.GetBestellingWithDetailsAsync(7)).ReturnsAsync((Bestelling)null!);

        Assert.That(await _service.WerkBestellingBijAsync(7, MaakVm(regels: (1, 1))), Is.False);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task WerkBestellingBijAsync_SynchroniseertRegelsEnLeverkosten()
    {
        Bestelling bestelling = MaakBestelling();
        _bestellingRepo.Setup(x => x.GetBestellingWithDetailsAsync(7)).ReturnsAsync(bestelling);

        // Mocha verdwijnt, Espresso gaat naar 20 stuks (€ 40 => gratis levering)
        bool result = await _service.WerkBestellingBijAsync(7, MaakVm("Afgerond", (1, 20), (2, 0)));

        Assert.That(result, Is.True);
        Assert.That(bestelling.Status, Is.EqualTo("Afgerond"));
        Assert.That(bestelling.KlantVoorNaam, Is.EqualTo("Jan"));
        Assert.That(bestelling.KoffiesInBestelling, Has.Count.EqualTo(1));
        Assert.That(bestelling.KoffiesInBestelling[0].Aantal, Is.EqualTo(20));
        Assert.That(bestelling.Leverkosten, Is.EqualTo(0m));
        _besteldeKoffiesRepo.Verify(x => x.Delete(It.Is<BesteldeKoffie>(b => b.KoffieId == 2)), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task WerkBestellingBijAsync_NieuweKoffie_WordtToegevoegd()
    {
        Bestelling bestelling = new() { Id = 7, KoffiesInBestelling = { new BesteldeKoffie { KoffieId = 1, Koffie = _koffies[0], Aantal = 1 } } };
        _bestellingRepo.Setup(x => x.GetBestellingWithDetailsAsync(7)).ReturnsAsync(bestelling);

        await _service.WerkBestellingBijAsync(7, MaakVm(regels: new[] { (1, 1), (2, 1) }));

        Assert.That(bestelling.KoffiesInBestelling.Select(x => x.KoffieId), Is.EquivalentTo(new[] { 1, 2 }));
        Assert.That(bestelling.Leverkosten, Is.EqualTo(5m));
    }

    [Test]
    public async Task VerwijderBestellingAsync_Onbestaand_GeeftFalse()
    {
        _bestellingRepo.Setup(x => x.GetBestellingWithDetailsAsync(7)).ReturnsAsync((Bestelling)null!);

        Assert.That(await _service.VerwijderBestellingAsync(7), Is.False);
    }

    [Test]
    public async Task VerwijderBestellingAsync_VerwijdertBestellingEnRegels()
    {
        Bestelling bestelling = MaakBestelling();
        _bestellingRepo.Setup(x => x.GetBestellingWithDetailsAsync(7)).ReturnsAsync(bestelling);

        Assert.That(await _service.VerwijderBestellingAsync(7), Is.True);
        _besteldeKoffiesRepo.Verify(x => x.Delete(It.IsAny<BesteldeKoffie>()), Times.Exactly(2));
        _bestellingRepo.Verify(x => x.Delete(bestelling), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
