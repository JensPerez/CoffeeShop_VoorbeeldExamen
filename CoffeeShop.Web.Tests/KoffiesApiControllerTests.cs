using Mapster;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CoffeeShop.Web.Configuration;
using CoffeeShop.Web.Controllers.Api;
using CoffeeShop.Web.Models;
using CoffeeShop.Web.Repositories;
using CoffeeShop.Web.Services;
using CoffeeShop.Web.ViewModels;

namespace CoffeeShop.Web.Tests;

[TestFixture]
public class KoffiesApiControllerTests
{
    private Mock<IKoffieService> _koffieService;
    private KoffiesApiController _controller;

    [OneTimeSetUp]
    public void OneTimeSetUp() => TypeAdapterConfig.GlobalSettings.Scan(typeof(MapperProfile).Assembly);

    [SetUp]
    public void SetUp()
    {
        _koffieService = new Mock<IKoffieService>();
        _koffieService.Setup(x => x.ValideerIngredientenAsync(It.IsAny<List<int>>())).ReturnsAsync((string?)null);

        _controller = new KoffiesApiController(Mock.Of<IUnitOfWork>(), _koffieService.Object);
    }

    [Test]
    public async Task GetKoffie_OngeldigId_GeeftBadRequest()
    {
        var result = await _controller.GetKoffie(0);

        Assert.That(result.Result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task CreateKoffie_Geldig_GeeftCreatedMetNieuwId()
    {
        var vm = new KoffieFormVm { Naam = "Nieuw", IngredientIds = new() { 1 } };
        _koffieService.Setup(x => x.MaakKoffieAsync(vm)).ReturnsAsync(new Koffie { Id = 13, Naam = "Nieuw" });

        var result = await _controller.CreateKoffie(vm);

        var created = result.Result as CreatedAtActionResult;
        Assert.That(created?.RouteValues?["id"], Is.EqualTo(13));
    }

    [Test]
    public async Task CreateKoffie_OngeldigeIngredienten_GeeftBadRequest()
    {
        _koffieService.Setup(x => x.ValideerIngredientenAsync(It.IsAny<List<int>>())).ReturnsAsync("fout");

        var result = await _controller.CreateKoffie(new KoffieFormVm());

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        _koffieService.Verify(x => x.MaakKoffieAsync(It.IsAny<KoffieFormVm>()), Times.Never);
    }

    [Test]
    public async Task UpdateKoffie_IdKomtNietOvereen_GeeftBadRequest()
    {
        Assert.That(await _controller.UpdateKoffie(1, new KoffieFormVm { Id = 2 }), Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task UpdateKoffie_OngeldigeIngredienten_GeeftBadRequest()
    {
        _koffieService.Setup(x => x.ValideerIngredientenAsync(It.IsAny<List<int>>())).ReturnsAsync("fout");

        Assert.That(await _controller.UpdateKoffie(5, new KoffieFormVm { Id = 5 }), Is.InstanceOf<BadRequestObjectResult>());
        _koffieService.Verify(x => x.WerkKoffieBijAsync(It.IsAny<int>(), It.IsAny<KoffieFormVm>()), Times.Never);
    }

    [TestCase(true, typeof(NoContentResult))]
    [TestCase(false, typeof(NotFoundResult))]
    public async Task UpdateKoffie_GeeftResultaatVanService(bool bijgewerkt, Type verwacht)
    {
        _koffieService.Setup(x => x.WerkKoffieBijAsync(5, It.IsAny<KoffieFormVm>())).ReturnsAsync(bijgewerkt);

        Assert.That(await _controller.UpdateKoffie(5, new KoffieFormVm { Id = 5 }), Is.InstanceOf(verwacht));
    }

    [TestCase(KoffieVerwijderResultaat.Verwijderd, typeof(NoContentResult))]
    [TestCase(KoffieVerwijderResultaat.NietGevonden, typeof(NotFoundResult))]
    [TestCase(KoffieVerwijderResultaat.Besteld, typeof(ConflictObjectResult))]
    public async Task DeleteKoffie_VertaaltResultaatVanService(KoffieVerwijderResultaat resultaat, Type verwacht)
    {
        _koffieService.Setup(x => x.VerwijderKoffieAsync(5)).ReturnsAsync(resultaat);

        Assert.That(await _controller.DeleteKoffie(5), Is.InstanceOf(verwacht));
    }
}
