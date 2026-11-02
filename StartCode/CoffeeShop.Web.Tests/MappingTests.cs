using Mapster;
using CoffeeShop.Web.Configuration;
using CoffeeShop.Web.Models;
using CoffeeShop.Web.ViewModels;

namespace CoffeeShop.Web.Tests;

[TestFixture]
public class MappingTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // Wordt exact één keer uitgevoerd voordat de tests in deze klasse starten.
        // Dit scant je MapperProfile en registreert alle mappings globaal.
        TypeAdapterConfig.GlobalSettings.Scan(typeof(MapperProfile).Assembly);
    }

    [Test]
    public void KoffieToBesteldeKoffieVm_Mapping_IsValid()
    {
        // Arrange
        var bestelling = new BesteldeKoffie
        {
            Id = 1,
            KoffieId = 10,
            Koffie = new Koffie
            {
                Id = 10,
                Naam = "Espresso",
                Prijs = 8.50m
            },
            BestellingId = 100,
            Aantal = 2,
            Bestelling = new Bestelling
            {
                Id = 100,
                BestelDatum = new DateTime(2024, 6, 1),
                Status = "In behandeling",
                Leverkosten = 2.50m,
            }
        };

        // Act
        BesteldeKoffieVm result = bestelling.Adapt<BesteldeKoffieVm>();

        // Assert
        Assert.That(result.KoffieNaam, Is.EqualTo(bestelling.Koffie.Naam));
        Assert.That(result.Aantal, Is.EqualTo(bestelling.Aantal));
        Assert.That(result.PrijsPerStuk, Is.EqualTo(bestelling.Koffie.Prijs));
    }

    [Test]
    public void BesteldeKoffie_Prijzen_Verzending_Totaal_WordenCorrectBerekend()
    {
        // Arrange
        var bestelling = new BesteldeKoffie
        {
            Id = 1,
            KoffieId = 10,
            Koffie = new Koffie
            {
                Id = 10,
                Naam = "Espresso",
                Prijs = 8.50m
            },
            BestellingId = 100,
            Aantal = 2,
            Bestelling = new Bestelling
            {
                Id = 100,
                Leverkosten = 2.50m,
            }
        };

        // Act
        BesteldeKoffieVm result = bestelling.Adapt<BesteldeKoffieVm>();

        // Assert
        Assert.That(result.Aantal, Is.EqualTo(2));
        Assert.That(result.PrijsPerStuk, Is.EqualTo(8.50m));
        Assert.That(result.Subtotaal, Is.EqualTo(17.0m));
    }

    [Test]
    public void Koffie_To_KoffieDetailVm_ShouldMapCorrectly_IncludingNestedIngredients()
    {
        // Arrange
        var koffie = new Koffie
        {
            Id = 1,
            Naam = "Cappuccino",
            Beschrijving = "Romig en vol",
            Prijs = 11.50m,
            IngredientenInKoffie = new List<IngredientKoffie>
            {
                new IngredientKoffie
                {
                    Ingredient = new Ingredient
                    {
                        Id = 5,
                        Naam = "Havermelk",
                        AantalInStock = 15,
                        Categorie = new Categorie { Id = 2, Naam = "Melk & Alternatieven" }
                    }
                }
            }
        };

        // Act
        var result = koffie.Adapt<KoffieDetailVm>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Naam, Is.EqualTo("Cappuccino"));
        Assert.That(result.Beschrijving, Is.EqualTo("Romig en vol"));
        Assert.That(result.Prijs, Is.EqualTo(11.50m));
        Assert.That(result.ImageUrl, Is.EqualTo("/img/Cappuccino.png"));

        // Controleer of de geneste lijst van ingrediënten correct is gemapt
        Assert.That(result.Ingredienten, Has.Count.EqualTo(1));
        Assert.That(result.Ingredienten[0].Naam, Is.EqualTo("Havermelk"));
        Assert.That(result.Ingredienten[0].AantalInStock, Is.EqualTo(15));
        Assert.That(result.Ingredienten[0].CategorieNaam, Is.EqualTo("Melk & Alternatieven"));
    }

    [Test]
    public void Ingredient_To_IngredientVm_ShouldMapCategoryNameCorrectly()
    {
        // Arrange
        var ingredient = new Ingredient
        {
            Id = 12,
            Naam = "Vanillesiroop",
            AantalInStock = 50,
            IsVegan = true,
            Categorie = new Categorie
            {
                Id = 3,
                Naam = "Siropen & Smaken"
            }
        };

        // Act
        var result = ingredient.Adapt<IngredientVm>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(12));
        Assert.That(result.Naam, Is.EqualTo("Vanillesiroop"));
        Assert.That(result.AantalInStock, Is.EqualTo(50));
        Assert.That(result.IsVegan, Is.True);
        Assert.That(result.CategorieNaam, Is.EqualTo("Siropen & Smaken"));
    }

    [Test]
    public void KoffieListItem_Mapping_ShouldMapImageUrlCorrectly()
    {
        // Arrange
        var koffie = new Koffie
        {
            Id = 2,
            Naam = "Americano",
            Prijs = 9.00m
        };

        // Act
        var result = koffie.Adapt<KoffieListItemVm>();

        // Assert
        Assert.That(result.Naam, Is.EqualTo("Americano"));
        Assert.That(result.ImageUrl, Is.EqualTo("/img/Americano.png"));
    }

    [Test]
    public void ListKoffie_To_KoffieListVm_ShouldMapCorrectly()
    {
        // Arrange
        var koffies = new List<Koffie>
        {
            new Koffie { Id = 1, Naam = "Espresso", Prijs = 8.00m },
            new Koffie { Id = 2, Naam = "Flat White", Prijs = 10.00m }
        };

        // Act
        var result = koffies.Adapt<KoffieListVm>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Koffies, Has.Count.EqualTo(2));
        Assert.That(result.Koffies[0].Naam, Is.EqualTo("Espresso"));
        Assert.That(result.Koffies[1].Naam, Is.EqualTo("Flat White"));
    }

    [Test]
    public void Bestelling_To_BestellingOverzichtVm_ShouldMapCustomerNameAndNestedItems()
    {
        // Arrange
        var bestelling = new Bestelling
        {
            Id = 1,
            KlantVoorNaam = "Jan",
            KlantAchterNaam = "Janssens",
            BestelDatum = new DateTime(2026, 3, 30),
            Status = "In behandeling",
            KoffiesInBestelling = new List<BesteldeKoffie>
            {
                new BesteldeKoffie
                {
                    Id = 1,
                    KoffieId = 1,
                    Aantal = 2,
                    Koffie = new Koffie { Id = 1, Naam = "Caffè Latte", Prijs = 9.50m }
                }
            }
        };

        // Act
        var result = bestelling.Adapt<BestellingOverzichtVm>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.KlantNaam, Is.EqualTo("Jan Janssens"));
        Assert.That(result.Status, Is.EqualTo("In behandeling"));
        Assert.That(result.BesteldeKoffies, Has.Count.EqualTo(1));
        Assert.That(result.BesteldeKoffies[0].KoffieNaam, Is.EqualTo("Caffè Latte"));
    }
}