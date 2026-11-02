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

    // GET: api/bestellingen/5
    // TODO: Geef één bestelling terug met de bestelde koffies en de totaalprijs.
    //       404 als de bestelling niet bestaat.
    [HttpGet("{id}")]
    public async Task<ActionResult<BestellingOverzichtVm>> GetBestellingById(int id)
    {
        throw new NotImplementedException();
    }

    // GET: api/bestellingen
    // TODO: Geef alle bestellingen terug, met de bestelde koffies en de totaalprijs (nieuwste eerst).
    [HttpGet]
    public async Task<ActionResult<List<BestellingOverzichtVm>>> GetAlleBestellingen()
    {
        throw new NotImplementedException();
    }

    // POST: api/bestellingen
    // TODO: Maak een nieuwe bestelling aan en geef 201 Created terug (met een verwijzing naar GetBestellingById).
    //       400 bij ongeldige regels of onvoldoende voorraad.
    [HttpPost]
    public async Task<ActionResult<BestellingCreatedVm>> CreateBestelling(BestellingFormVm vm)
    {
        throw new NotImplementedException();
    }
}
