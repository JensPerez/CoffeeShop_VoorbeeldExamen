using CoffeeShop.Web.Services;

namespace CoffeeShop.Web.Controllers.Api;

[Route("api/koffies")]
[ApiController]
public class KoffiesApiController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public KoffiesApiController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET: api/koffies
    // GET: api/koffies?zoekWaarde=cappuccino
    // TODO: Geef alle koffies terug, of enkel de koffies waarvan de naam de zoekwaarde bevat.
    [HttpGet]
    public async Task<ActionResult<KoffieListVm>> GetKoffies([FromQuery] string? zoekWaarde)
    {
        throw new NotImplementedException();
    }

    // GET: api/koffies/5
    // TODO: Geef de details van één koffie terug, inclusief ingrediënten en of ze vegan is.
    //       400 bij een ongeldig id (<= 0), 404 als de koffie niet bestaat.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<KoffieDetailVm>> GetKoffie(int id)
    {
        throw new NotImplementedException();
    }

    // POST: api/koffies
    // TODO: Maak een nieuwe koffie aan en geef 201 Created terug (met een verwijzing naar GetKoffie).
    //       400 bij ongeldige ingrediënten.
    [HttpPost]
    public async Task<ActionResult<KoffieListItemVm>> CreateKoffie(KoffieFormVm vm)
    {
        throw new NotImplementedException();
    }

    // PUT: api/koffies/5
    // TODO: Werk de koffie bij en geef 204 No Content terug.
    //       400 als het id niet overeenkomt of bij ongeldige ingrediënten, 404 als de koffie niet bestaat.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateKoffie(int id, KoffieFormVm vm)
    {
        throw new NotImplementedException();
    }
}
