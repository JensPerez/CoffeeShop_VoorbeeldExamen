using CoffeeShop.Web.Services;

namespace CoffeeShop.Web.Controllers;

public class BestellingController : Controller
{
    private readonly IUnitOfWork _uow;
    private readonly IBestellingService _bestellingService;

    public BestellingController(IUnitOfWork uow, IBestellingService bestellingService)
    {
        _uow = uow;
        _bestellingService = bestellingService;
    }

    public async Task<IActionResult> Index()
    {
        List<Bestelling> bestellingen = await _uow.BestellingRepo.GetAllWithDetailsAsync();

        List<BestellingOverzichtVm> vm = bestellingen.Adapt<List<BestellingOverzichtVm>>();

        foreach (BestellingOverzichtVm bestelling in vm)
        {
            bestelling.TotaalPrijs = _bestellingService.BerekenTotaal(bestelling) + bestelling.Leverkosten;
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? koffieId)
    {
        BestellingFormVm vm = new BestellingFormVm();
        await VulRegelsAsync(vm);

        // Vooraf geselecteerde koffie (bv. via "Bestel deze koffie" op de detailpagina)
        BestelRegelVm? regel = vm.Regels.FirstOrDefault(x => x.KoffieId == koffieId);
        if (regel != null)
        {
            regel.Aantal = 1;
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BestellingFormVm vm)
    {
        await ValideerRegelsAsync(vm);

        if (ModelState.IsValid)
        {
            try
            {
                BestellingCreatedVm resultaat = await _bestellingService.MaakBestellingAsync(vm);

                TempData["Melding"] = $"Bestelling #{resultaat.Id} voor {resultaat.KlantNaam} is geplaatst.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        await VulRegelsAsync(vm);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        Bestelling bestelling = await _uow.BestellingRepo.GetBestellingWithDetailsAsync(id);

        if (bestelling == null)
        {
            return NotFound();
        }

        BestellingFormVm vm = new BestellingFormVm
        {
            Id = bestelling.Id,
            KlantVoorNaam = bestelling.KlantVoorNaam,
            KlantAchterNaam = bestelling.KlantAchterNaam,
            Status = bestelling.Status
        };

        await VulRegelsAsync(vm);

        foreach (BestelRegelVm regel in vm.Regels)
        {
            regel.Aantal = bestelling.KoffiesInBestelling
                .Where(x => x.KoffieId == regel.KoffieId)
                .Sum(x => x.Aantal);
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BestellingFormVm vm)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        await ValideerRegelsAsync(vm);

        if (!BestellingFormVm.Statussen.Contains(vm.Status))
        {
            ModelState.AddModelError(nameof(vm.Status), "Ongeldige status.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (!await _bestellingService.WerkBestellingBijAsync(id, vm))
                {
                    return NotFound();
                }

                TempData["Melding"] = $"Bestelling #{id} is bijgewerkt.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        await VulRegelsAsync(vm);
        return View(vm);
    }

    private async Task ValideerRegelsAsync(BestellingFormVm vm)
    {
        string? fout = await _bestellingService.ValideerRegelsAsync(vm.Regels);

        if (fout != null)
        {
            ModelState.AddModelError(string.Empty, fout);
        }
    }

    // Zorgt dat er voor elke koffie een regel is, met naam en prijs uit de database
    private async Task VulRegelsAsync(BestellingFormVm vm)
    {
        List<Koffie> koffies = await _uow.KoffieRepo.GetAllAsync();

        vm.Regels = koffies
            .OrderBy(x => x.Naam)
            .Select(koffie => new BestelRegelVm
            {
                KoffieId = koffie.Id,
                KoffieNaam = koffie.Naam,
                Prijs = koffie.Prijs,
                Aantal = vm.Regels.FirstOrDefault(x => x.KoffieId == koffie.Id)?.Aantal ?? 0
            })
            .ToList();
    }
}
