using Moq;
using CoffeeShop.Web.Data;
using CoffeeShop.Web.Models;
using CoffeeShop.Web.Repositories;
using CoffeeShop.Web.Services;
using CoffeeShop.Web.ViewModels;

namespace CoffeeShop.Web.Tests;

[TestFixture]
public class BestellingServiceTests
{
    private Mock<IUnitOfWork> _mockUow;
    private Mock<IIngredientRepo> _mockIngredientRepo;
    private BestellingService _bestellingService;

    [SetUp]
    public void SetUp()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockIngredientRepo = new Mock<IIngredientRepo>();

        // Koppel de repository aan de UnitOfWork mock
        _mockUow.Setup(u => u.IngredientRepo).Returns(_mockIngredientRepo.Object);

        _bestellingService = new BestellingService(_mockUow.Object);
    }

    [Test]
    public async Task PlaatsBestellingAsync_OnderDrempel_BerekentCorrecteLeverkosten()
    {
        // Arrange: Subtotaal = 2 * 10.00 = 20.00 (Onder de drempel van 30.00)
        var bestelling = new Bestelling
        {
            Id = 1,
            KlantVoorNaam = "Jan",
            KlantAchterNaam = "Janssens",
            Leverkosten = 5.00m,
            KoffiesInBestelling = new List<BesteldeKoffie>
            {
                new BesteldeKoffie
                {
                    Aantal = 2,
                    Koffie = new Koffie { Id = 1, Prijs = 10.00m }
                }
            }
        };

        // Geen voorraadproblemen verwachten
        _mockIngredientRepo.Setup(r => r.GetIngredientenForKoffieAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Ingredient>());

        // Act
        var result = await _bestellingService.PlaatsBestellingAsync(bestelling);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.KlantNaam, Is.EqualTo("Jan Janssens"));
        Assert.That(result.Subtotaal, Is.EqualTo(20.00m));
        Assert.That(result.Leverkosten, Is.EqualTo(5.00m));
        Assert.That(result.Totaal, Is.EqualTo(25.00m)); // 20.00 + 5.00
    }

    [Test]
    public async Task PlaatsBestellingAsync_BovenOfGelijkAanDrempel_BerekentGratisVerzending()
    {
        // Arrange: Subtotaal = 3 * 10.00 = 30.00 (Exact op de gratis verzending drempel)
        var bestelling = new Bestelling
        {
            Id = 2,
            KlantVoorNaam = "Els",
            KlantAchterNaam = "Peeters",
            Leverkosten = 5.00m, // Wordt genegeerd
            KoffiesInBestelling = new List<BesteldeKoffie>
            {
                new BesteldeKoffie
                {
                    Aantal = 3,
                    Koffie = new Koffie { Id = 1, Prijs = 10.00m }
                }
            }
        };

        _mockIngredientRepo.Setup(r => r.GetIngredientenForKoffieAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Ingredient>());

        // Act
        var result = await _bestellingService.PlaatsBestellingAsync(bestelling);

        // Assert
        Assert.That(result.Subtotaal, Is.EqualTo(30.00m));
        Assert.That(result.Leverkosten, Is.EqualTo(0.00m)); // Gratis verzending
        Assert.That(result.Totaal, Is.EqualTo(30.00m));
    }

    [Test]
    public async Task PlaatsBestellingAsync_OnvoldoendeVoorraad_GooitInvalidOperationException()
    {
        // Arrange
        var bestelling = new Bestelling
        {
            Id = 3,
            KlantVoorNaam = "Piet",
            KlantAchterNaam = "Smet",
            KoffiesInBestelling = new List<BesteldeKoffie>
            {
                new BesteldeKoffie
                {
                    KoffieId = 5,
                    Aantal = 5, // Vraagt 5 stuks
                    Koffie = new Koffie { Id = 5, Prijs = 12.00m }
                }
            }
        };

        // Maar er zijn er slechts 2 in stock!
        var ingredienten = new List<Ingredient>
        {
            new Ingredient { Id = 1, Naam = "Volle Melk", AantalInStock = 2 }
        };

        _mockIngredientRepo.Setup(r => r.GetIngredientenForKoffieAsync(It.IsAny<int>()))
            .ReturnsAsync(ingredienten);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _bestellingService.PlaatsBestellingAsync(bestelling));

        Assert.That(ex.Message, Does.Contain("Onvoldoende voorraad voor ingrediënt 'Volle Melk'"));
    }

    [Test]
    public async Task PlaatsBestellingAsync_VoldoendeVoorraad_SlaagtZonderFouten()
    {
        // Arrange
        var bestelling = new Bestelling
        {
            Id = 4,
            KlantVoorNaam = "Anna",
            KlantAchterNaam = "De Wit",
            KoffiesInBestelling = new List<BesteldeKoffie>
            {
                new BesteldeKoffie
                {
                    KoffieId = 2,
                    Aantal = 2,
                    Koffie = new Koffie { Id = 2, Prijs = 9.00m }
                }
            }
        };

        var ingredienten = new List<Ingredient>
        {
            new Ingredient { Id = 1, Naam = "Espressobonen", AantalInStock = 10 }
        };

        _mockIngredientRepo.Setup(r => r.GetIngredientenForKoffieAsync(It.IsAny<int>()))
            .ReturnsAsync(ingredienten);

        // Act
        var result = await _bestellingService.PlaatsBestellingAsync(bestelling);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Totaal, Is.EqualTo(23.00m)); // (18.00 subtotaal + 5.00 leverkosten)
    }

    [Test]
    public void BerekenTotaal_TeltSubtotalenCorrectOp()
    {
        // Arrange
        var vm = new BestellingOverzichtVm
        {
            BesteldeKoffies = new List<BesteldeKoffieVm>
            {
                new BesteldeKoffieVm { Subtotaal = 12.50m },
                new BesteldeKoffieVm { Subtotaal = 8.00m },
                new BesteldeKoffieVm { Subtotaal = 15.25m }
            }
        };

        // Act
        decimal totaal = _bestellingService.BerekenTotaal(vm);

        // Assert
        Assert.That(totaal, Is.EqualTo(35.75m));
    }

    [Test]
    public async Task BerekenTotaalBedrag_ZonderKorting_BerekentSubtotaalPlusLeverkosten()
    {
        // Arrange
        var bestelling = new Bestelling
        {
            KoffiesInBestelling = new List<BesteldeKoffie>
            {
                new BesteldeKoffie
                {
                    Id = 1,
                    Aantal = 2,
                    Koffie = new Koffie { Naam = "Espresso", Prijs = 10.00m }
                }
            }
        };

        // Geen voorraadproblemen verwachten
        _mockIngredientRepo.Setup(r => r.GetIngredientenForKoffieAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Ingredient>());

        var service = new BestellingService(_mockUow.Object);

        // Act
        BestellingCreatedVm result = await service.PlaatsBestellingAsync(bestelling);

        Assert.That(result.Subtotaal, Is.EqualTo(20.0m));
        Assert.That(result.Leverkosten, Is.EqualTo(5m));
        Assert.That(result.Totaal, Is.EqualTo(25m));
    }

    [Test]
    public async Task BerekenTotaalBedrag_MetGratisVerzending_VanafDrempelBedragAsync()
    {
        // Arrange (Drempel is €30,00)
        var bestelling = new Bestelling
        {
            Leverkosten = 3.50m,
            KoffiesInBestelling = new List<BesteldeKoffie>
            {
                new BesteldeKoffie
                {
                    Id = 1,
                    Aantal = 3,
                    Koffie = new Koffie { Naam = "Cappuccino", Prijs = 12.00m }
                } // Subtotaal = 36.00
            }
        };

        // Geen voorraadproblemen verwachten
        _mockIngredientRepo.Setup(r => r.GetIngredientenForKoffieAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Ingredient>());
        var service = new BestellingService(_mockUow.Object);

        // Act
        BestellingCreatedVm result = await service.PlaatsBestellingAsync(bestelling);

        // Assert: Subtotaal 36.00, leverkosten vervallen
        Assert.That(result.Subtotaal, Is.EqualTo(36.00m));
        Assert.That(result.Leverkosten, Is.EqualTo(0.00m));
        Assert.That(result.Totaal, Is.EqualTo(36.00m));
    }

    [Test]
    public async Task PlaatsBestelling_GooitException_WanneerIngredientOnvoldoendeVoorraad()
    {
        // Arrange
        var melkIngredienten = new List<Ingredient> { new Ingredient { Naam = "Espressobonen", AantalInStock = 20 }, new Ingredient { Naam = "Volle Melk", AantalInStock = 2 } };

        var bestelling = new Bestelling
        {
            KoffiesInBestelling = new List<BesteldeKoffie>
            {
                new BesteldeKoffie
                {
                    Id = 1,
                    Aantal = 3, // Vraagt om 3 stuks, maar er zijn er maar 2
                }
            }
        };

        _mockUow.Setup(x => x.IngredientRepo.GetIngredientenForKoffieAsync(It.IsAny<int>()))
        .ReturnsAsync(melkIngredienten);

        var service = new BestellingService(_mockUow.Object);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await service.PlaatsBestellingAsync(bestelling));
        Assert.That(ex.Message, Does.Contain("Volle Melk"));
    }
}