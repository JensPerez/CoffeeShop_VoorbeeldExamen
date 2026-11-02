using Mapster;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Web.Services;

namespace CoffeeShop.Web.Controllers;

public class KoffieController : Controller
{
    private IUnitOfWork _uow;
    private IVeganService _veganService;

    public KoffieController(IUnitOfWork uow, IVeganService veganService)
    {
        _uow = uow;
        _veganService = veganService;
    }

    // TODO: Haal de koffies op uit de database (optioneel gefilterd op zoekWaarde)
    //       en geef een ViewModel door aan de view.
    public IActionResult Index(string zoekWaarde)
    {
        return View();
    }

    // TODO: Haal de koffie met het gegeven id op (inclusief ingrediënten en hun categorie),
    //       bepaal of ze vegan is en geef een ViewModel door aan de view.
    //       Geef NotFound() terug als de koffie niet bestaat.
    public IActionResult Details(int id)
    {
        return View();
    }
}
