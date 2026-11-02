using Mapster;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Web.Services;

namespace CoffeeShop.Web.Controllers;

public class KoffieController : Controller
{
    private IUnitOfWork _uow;
    private IVeganService _veganService;
    private IKoffieService _koffieService;

    public KoffieController(IUnitOfWork uow, IVeganService veganService, IKoffieService koffieService)
    {
        _uow = uow;
        _veganService = veganService;
        _koffieService = koffieService;
    }

    public async Task<IActionResult> Index(string zoekWaarde)
    {
        List<Koffie> koffies;

        if (string.IsNullOrWhiteSpace(zoekWaarde))
        {
            koffies = await _uow.KoffieRepo.GetAllAsync();
        }
        else
        {
            koffies = await _uow.KoffieRepo.GetByName(zoekWaarde);
        }

        KoffieListVm vm = koffies.Adapt<KoffieListVm>();
        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        Koffie koffie = await _uow.KoffieRepo.GetKoffieDetailAsync(id);

        if (koffie == null)
        {
            return NotFound();
        }

        KoffieDetailVm vm = koffie.Adapt<KoffieDetailVm>();
        vm.IsVegan = _veganService.BepaalVegan(vm.Ingredienten);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        KoffieFormVm vm = new KoffieFormVm();
        await VulIngredientenAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KoffieFormVm vm)
    {
        await ValideerIngredientenAsync(vm);

        if (!ModelState.IsValid)
        {
            await VulIngredientenAsync(vm);
            return View(vm);
        }

        Koffie koffie = await _koffieService.MaakKoffieAsync(vm);

        return RedirectToAction(nameof(Details), new { id = koffie.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        Koffie koffie = await _uow.KoffieRepo.GetKoffieDetailAsync(id);

        if (koffie == null)
        {
            return NotFound();
        }

        KoffieFormVm vm = koffie.Adapt<KoffieFormVm>();
        await VulIngredientenAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, KoffieFormVm vm)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        await ValideerIngredientenAsync(vm);

        if (!ModelState.IsValid)
        {
            await VulIngredientenAsync(vm);
            return View(vm);
        }

        if (!await _koffieService.WerkKoffieBijAsync(id, vm))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task VulIngredientenAsync(KoffieFormVm vm)
    {
        List<Ingredient> ingredienten = await _uow.IngredientRepo.GetAllWithCategorieAsync();
        vm.BeschikbareIngredienten = ingredienten.Adapt<List<IngredientVm>>();
    }

    private async Task ValideerIngredientenAsync(KoffieFormVm vm)
    {
        string? fout = await _koffieService.ValideerIngredientenAsync(vm.IngredientIds);

        if (fout != null)
        {
            ModelState.AddModelError(nameof(vm.IngredientIds), fout);
        }
    }
}