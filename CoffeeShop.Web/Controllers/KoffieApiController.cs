using CoffeeShop.Web.Services;

namespace CoffeeShop.Web.Controllers.Api;

[Route("api/koffies")]
[ApiController]
public class KoffiesApiController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IKoffieService _koffieService;

    public KoffiesApiController(IUnitOfWork uow, IKoffieService koffieService)
    {
        _uow = uow;
        _koffieService = koffieService;
    }

    // GET: api/koffies
    // GET: api/koffies?zoekWaarde=cappuccino
    [HttpGet]
    public async Task<ActionResult<KoffieListVm>> GetKoffies([FromQuery] string? zoekWaarde)
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
        return Ok(vm);
    }

    // GET: api/koffies/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<KoffieDetailVm>> GetKoffie(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        Koffie koffie = await _uow.KoffieRepo.GetKoffieDetailAsync(id);

        if (koffie == null)
        {
            return NotFound();
        }

        KoffieDetailVm vm = koffie.Adapt<KoffieDetailVm>();
        vm.IsVegan = BepaalVegan(vm.Ingredienten);

        return Ok(vm);
    }

    // POST: api/koffies
    [HttpPost]
    public async Task<ActionResult<KoffieListItemVm>> CreateKoffie(KoffieFormVm vm)
    {
        if (!await IngredientenGeldigAsync(vm))
        {
            return BadRequest(ModelState);
        }

        Koffie koffie = await _koffieService.MaakKoffieAsync(vm);

        return CreatedAtAction(nameof(GetKoffie), new { id = koffie.Id }, koffie.Adapt<KoffieListItemVm>());
    }

    // PUT: api/koffies/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateKoffie(int id, KoffieFormVm vm)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!await IngredientenGeldigAsync(vm))
        {
            return BadRequest(ModelState);
        }

        return await _koffieService.WerkKoffieBijAsync(id, vm) ? NoContent() : NotFound();
    }

    // DELETE: api/koffies/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteKoffie(int id)
    {
        return await _koffieService.VerwijderKoffieAsync(id) switch
        {
            KoffieVerwijderResultaat.NietGevonden => NotFound(),
            KoffieVerwijderResultaat.Besteld => Conflict("Deze koffie komt voor in bestellingen en kan niet verwijderd worden."),
            _ => NoContent()
        };
    }

    private async Task<bool> IngredientenGeldigAsync(KoffieFormVm vm)
    {
        string? fout = await _koffieService.ValideerIngredientenAsync(vm.IngredientIds);

        if (fout != null)
        {
            ModelState.AddModelError(nameof(vm.IngredientIds), fout);
        }

        return fout == null;
    }

    private bool BepaalVegan(List<IngredientVm> ingredienten)
    {
        return ingredienten.All(x => x.IsVegan);
    }
}