namespace CoffeeShop.Web.Services
{
    public interface IBestellingService
    {
        decimal BerekenTotaal(BestellingOverzichtVm vm);
        Task<BestellingCreatedVm> PlaatsBestellingAsync(Bestelling bestelling);

        // Geeft een foutmelding terug, of null als de regels geldig zijn
        Task<string?> ValideerRegelsAsync(List<BestelRegelVm> regels);

        // Gooit InvalidOperationException bij onvoldoende voorraad
        Task<BestellingCreatedVm> MaakBestellingAsync(BestellingFormVm vm);

    }
}