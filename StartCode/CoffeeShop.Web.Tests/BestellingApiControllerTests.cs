using Microsoft.AspNetCore.Mvc;
using Moq;
using CoffeeShop.Web.Controllers.Api;
using CoffeeShop.Web.Models;
using CoffeeShop.Web.Repositories;
using CoffeeShop.Web.Services;
using CoffeeShop.Web.ViewModels;

namespace CoffeeShop.Web.Tests;

[TestFixture]
public class BestellingApiControllerTests
{
    private Mock<IBestellingRepo> _bestellingRepo;
    private Mock<IBestellingService> _service;
    private BestellingApiController _controller;

    [SetUp]
    public void SetUp()
    {
        var uow = new Mock<IUnitOfWork>();
        _bestellingRepo = new Mock<IBestellingRepo>();
        _service = new Mock<IBestellingService>();

        uow.Setup(x => x.BestellingRepo).Returns(_bestellingRepo.Object);
        _service.Setup(x => x.ValideerRegelsAsync(It.IsAny<List<BestelRegelVm>>())).ReturnsAsync((string?)null);

        _controller = new BestellingApiController(uow.Object, _service.Object);
    }

    private static BestellingFormVm MaakVm() => new();

    [Test]
    public async Task GetBestellingById_Onbestaand_GeeftNotFound()
    {
        _bestellingRepo.Setup(x => x.GetBestellingWithDetailsAsync(1)).ReturnsAsync((Bestelling)null!);

        var result = await _controller.GetBestellingById(1);

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task CreateBestelling_Geldig_GeeftCreatedMetResultaat()
    {
        var resultaat = new BestellingCreatedVm { Id = 11 };
        _service.Setup(x => x.MaakBestellingAsync(It.IsAny<BestellingFormVm>())).ReturnsAsync(resultaat);

        var result = await _controller.CreateBestelling(MaakVm());

        var created = result.Result as CreatedAtActionResult;
        Assert.That(created?.Value, Is.SameAs(resultaat));
        Assert.That(created?.RouteValues?["id"], Is.EqualTo(11));
    }

    [Test]
    public async Task CreateBestelling_OngeldigeRegels_GeeftBadRequest()
    {
        _service.Setup(x => x.ValideerRegelsAsync(It.IsAny<List<BestelRegelVm>>())).ReturnsAsync("fout");

        var result = await _controller.CreateBestelling(MaakVm());

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        _service.Verify(x => x.MaakBestellingAsync(It.IsAny<BestellingFormVm>()), Times.Never);
    }

    [Test]
    public async Task CreateBestelling_OnvoldoendeVoorraad_GeeftBadRequest()
    {
        _service.Setup(x => x.MaakBestellingAsync(It.IsAny<BestellingFormVm>())).ThrowsAsync(new InvalidOperationException("voorraad"));

        var result = await _controller.CreateBestelling(MaakVm());

        Assert.That((result.Result as BadRequestObjectResult)?.Value, Is.EqualTo("voorraad"));
    }

}
