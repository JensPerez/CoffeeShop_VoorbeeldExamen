using CoffeeShop.Web.Services;

namespace CoffeeShop.Web.Controllers.Api; 

[Route("api/bestellingen")]
[ApiController]
public class BestellingApiController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IBestellingService _bestellingService;

    public BestellingApiController(IUnitOfWork uow, IBestellingService bestellingService)
    {
        _uow = uow;
        _bestellingService = bestellingService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BestellingOverzichtVm>> GetBestellingById(int id) 
    {
        Bestelling bestelling = await _uow.BestellingRepo.GetBestellingWithDetailsAsync(id);

        if (bestelling == null)
        {
            return NotFound();
        }

        BestellingOverzichtVm vm = bestelling.Adapt<BestellingOverzichtVm>();
        vm.TotaalPrijs = _bestellingService.BerekenTotaal(vm);

        return Ok(vm);
    }

    [HttpGet]
    public async Task<ActionResult<List<BestellingOverzichtVm>>> GetAlleBestellingen()
    {
        var bestellingen = await _uow.BestellingRepo.GetAllAsync();

        if (bestellingen == null || !bestellingen.Any())
        {
            return NotFound();
        }

        List<BestellingOverzichtVm> vm = bestellingen.Adapt<List<BestellingOverzichtVm>>();

        return Ok(vm);
    }

    [HttpPost]
    public async Task<ActionResult<BestellingCreatedVm>> CreateBestelling(BestellingFormVm vm)
    {
        if (!await RegelsGeldigAsync(vm))
        {
            return BadRequest(ModelState);
        }

        try
        {
            BestellingCreatedVm resultaat = await _bestellingService.MaakBestellingAsync(vm);
            return CreatedAtAction(nameof(GetBestellingById), new { id = resultaat.Id }, resultaat);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Regels bevatten de volledige gewenste inhoud: ontbrekende koffies worden verwijderd
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBestelling(int id, BestellingFormVm vm)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!BestellingFormVm.Statussen.Contains(vm.Status))
        {
            ModelState.AddModelError(nameof(vm.Status), "Ongeldige status.");
        }

        if (!await RegelsGeldigAsync(vm) || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            return await _bestellingService.WerkBestellingBijAsync(id, vm) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBestelling(int id)
    {
        return await _bestellingService.VerwijderBestellingAsync(id) ? NoContent() : NotFound();
    }

    private async Task<bool> RegelsGeldigAsync(BestellingFormVm vm)
    {
        string? fout = await _bestellingService.ValideerRegelsAsync(vm.Regels);

        if (fout != null)
        {
            ModelState.AddModelError(nameof(vm.Regels), fout);
        }

        return fout == null;
    }
}