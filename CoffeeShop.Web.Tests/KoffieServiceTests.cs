using Mapster;
using Moq;
using CoffeeShop.Web.Configuration;
using CoffeeShop.Web.Data;
using CoffeeShop.Web.Models;
using CoffeeShop.Web.Repositories;
using CoffeeShop.Web.Services;
using CoffeeShop.Web.ViewModels;

namespace CoffeeShop.Web.Tests;

[TestFixture]
public class KoffieServiceTests
{
    private Mock<IUnitOfWork> _uow;
    private Mock<IKoffieRepo> _koffieRepo;
    private Mock<IGenericRepo<IngredientKoffie>> _ingredientKoffieRepo;
    private KoffieService _service;

    [OneTimeSetUp]
    public void OneTimeSetUp() => TypeAdapterConfig.GlobalSettings.Scan(typeof(MapperProfile).Assembly);

    [SetUp]
    public void SetUp()
    {
        _uow = new Mock<IUnitOfWork>();
        _koffieRepo = new Mock<IKoffieRepo>();
        _ingredientKoffieRepo = new Mock<IGenericRepo<IngredientKoffie>>();
        var ingredientRepo = new Mock<IIngredientRepo>();

        _uow.Setup(x => x.KoffieRepo).Returns(_koffieRepo.Object);
        _uow.Setup(x => x.IngredientRepo).Returns(ingredientRepo.Object);
        _uow.Setup(x => x.IngredientKoffieRepo).Returns(_ingredientKoffieRepo.Object);
        ingredientRepo.Setup(x => x.GetExistingIdsAsync()).ReturnsAsync(new[] { 1, 2, 3 });

        _service = new KoffieService(_uow.Object);
    }

    private static Koffie MaakKoffie() => new()
    {
        Id = 5,
        Naam = "Latte",
        Prijs = 3m,
        IngredientenInKoffie = { new IngredientKoffie { IngredientId = 1 }, new IngredientKoffie { IngredientId = 2 } }
    };

    [TestCase(new int[0], "minstens één")]
    [TestCase(new[] { 1, 99 }, "bestaan niet")]
    public async Task ValideerIngredientenAsync_Ongeldig_GeeftFoutmelding(int[] ids, string verwacht)
    {
        Assert.That(await _service.ValideerIngredientenAsync(ids.ToList()), Does.Contain(verwacht));
    }

    [Test]
    public async Task ValideerIngredientenAsync_Geldig_GeeftNull()
    {
        Assert.That(await _service.ValideerIngredientenAsync(new() { 1, 3 }), Is.Null);
    }

    [Test]
    public async Task MaakKoffieAsync_VoegtKoffieMetUniekeIngredientenToe()
    {
        var vm = new KoffieFormVm { Naam = "Nieuw", Prijs = 2.5m, IngredientIds = new() { 1, 3, 3 } };

        Koffie koffie = await _service.MaakKoffieAsync(vm);

        Assert.That(koffie.Naam, Is.EqualTo("Nieuw"));
        Assert.That(koffie.IngredientenInKoffie.Select(x => x.IngredientId), Is.EquivalentTo(new[] { 1, 3 }));
        _koffieRepo.Verify(x => x.Add(koffie), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task WerkKoffieBijAsync_Onbestaand_GeeftFalse()
    {
        _koffieRepo.Setup(x => x.GetKoffieDetailAsync(5)).ReturnsAsync((Koffie)null!);

        Assert.That(await _service.WerkKoffieBijAsync(5, new KoffieFormVm { Id = 5 }), Is.False);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task WerkKoffieBijAsync_WerktVeldenEnIngredientenBij()
    {
        Koffie koffie = MaakKoffie();
        _koffieRepo.Setup(x => x.GetKoffieDetailAsync(5)).ReturnsAsync(koffie);

        bool result = await _service.WerkKoffieBijAsync(5, new KoffieFormVm { Id = 5, Naam = "Latte XL", Prijs = 4m, IngredientIds = new() { 1, 3 } });

        Assert.That(result, Is.True);
        Assert.That(koffie.Naam, Is.EqualTo("Latte XL"));
        Assert.That(koffie.Prijs, Is.EqualTo(4m));
        Assert.That(koffie.IngredientenInKoffie.Select(x => x.IngredientId), Does.Contain(3));
        _ingredientKoffieRepo.Verify(x => x.Delete(It.Is<IngredientKoffie>(k => k.IngredientId == 2)), Times.Once);
        _ingredientKoffieRepo.Verify(x => x.Delete(It.Is<IngredientKoffie>(k => k.IngredientId == 1)), Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task VerwijderKoffieAsync_Onbestaand_GeeftNietGevonden()
    {
        _koffieRepo.Setup(x => x.GetKoffieDetailAsync(5)).ReturnsAsync((Koffie)null!);

        Assert.That(await _service.VerwijderKoffieAsync(5), Is.EqualTo(KoffieVerwijderResultaat.NietGevonden));
    }

    [Test]
    public async Task VerwijderKoffieAsync_Besteld_VerwijdertNiets()
    {
        _koffieRepo.Setup(x => x.GetKoffieDetailAsync(5)).ReturnsAsync(MaakKoffie());
        _koffieRepo.Setup(x => x.IsBesteldAsync(5)).ReturnsAsync(true);

        Assert.That(await _service.VerwijderKoffieAsync(5), Is.EqualTo(KoffieVerwijderResultaat.Besteld));
        _koffieRepo.Verify(x => x.Delete(It.IsAny<Koffie>()), Times.Never);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task VerwijderKoffieAsync_NietBesteld_VerwijdertKoffieEnKoppelingen()
    {
        Koffie koffie = MaakKoffie();
        _koffieRepo.Setup(x => x.GetKoffieDetailAsync(5)).ReturnsAsync(koffie);

        Assert.That(await _service.VerwijderKoffieAsync(5), Is.EqualTo(KoffieVerwijderResultaat.Verwijderd));
        _ingredientKoffieRepo.Verify(x => x.Delete(It.IsAny<IngredientKoffie>()), Times.Exactly(2));
        _koffieRepo.Verify(x => x.Delete(koffie), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
