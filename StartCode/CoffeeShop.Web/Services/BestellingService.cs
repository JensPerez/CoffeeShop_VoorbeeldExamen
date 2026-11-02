namespace CoffeeShop.Web.Services;

public class BestellingService : IBestellingService
{
    private IUnitOfWork _uow;

    public BestellingService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private const decimal GratisVerzendingDrempel = 30.00m;
    private const decimal LeverKosten = 5m;

    public async Task<BestellingCreatedVm> PlaatsBestellingAsync(Bestelling bestelling)
    {
        throw new NotImplementedException();
    }

    public decimal BerekenTotaal(BestellingOverzichtVm vm)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> ValideerRegelsAsync(List<BestelRegelVm> regels)
    {
        throw new NotImplementedException();
    }

    public async Task<BestellingCreatedVm> MaakBestellingAsync(BestellingFormVm vm)
    {
        throw new NotImplementedException();
    }
}
