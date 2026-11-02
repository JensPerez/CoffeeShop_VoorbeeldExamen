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
        await ValideerVoorraad(bestelling);

        decimal subtotaal = bestelling.KoffiesInBestelling?
            .Sum(bp => bp.Aantal * (bp.Koffie?.Prijs ?? 0m)) ?? 0m;

        // 3. Bepaal gratis verzending en leverkosten
        bool gratisVerzending = subtotaal >= GratisVerzendingDrempel;
        decimal leverkosten = gratisVerzending ? 0m : LeverKosten;
        decimal totaal = subtotaal + leverkosten;

        return new BestellingCreatedVm
        {
            Id = bestelling.Id,
            KlantNaam = $"{bestelling.KlantVoorNaam} {bestelling.KlantAchterNaam}",
            Subtotaal = subtotaal,
            Leverkosten = leverkosten,
            Totaal = totaal,
        };
    }

    private async Task ValideerVoorraad(Bestelling bestelling)
    {
        if (bestelling == null || bestelling.KoffiesInBestelling == null)
        {
            return;
        }

        foreach (var koffie in bestelling.KoffiesInBestelling)
        {
            List<Ingredient> nodigeIngredienten = await _uow.IngredientRepo.GetIngredientenForKoffieAsync(koffie.KoffieId);

            foreach (var ingredient in nodigeIngredienten)
            {
                if (koffie.Aantal > ingredient.AantalInStock)
                {
                    throw new InvalidOperationException(
                    $"Onvoldoende voorraad voor ingrediënt '{ingredient.Naam}'. Beschikbaar: {ingredient.AantalInStock}, Benodigd: {koffie.Aantal}.");
                }
            }
        }
    }

    public decimal BerekenTotaal(BestellingOverzichtVm vm)
    {
        return vm.BesteldeKoffies.Sum(x => x.Subtotaal);
    }

    public async Task<string?> ValideerRegelsAsync(List<BestelRegelVm> regels)
    {
        int[] bestaandeIds = await _uow.KoffieRepo.GetExistingIdsAsync();

        if (regels.Any(x => x.Aantal < 0 || !bestaandeIds.Contains(x.KoffieId)))
        {
            return "Ongeldige koffie of aantal.";
        }

        return regels.Any(x => x.Aantal > 0) ? null : "Kies minstens één koffie.";
    }

    public async Task<BestellingCreatedVm> MaakBestellingAsync(BestellingFormVm vm)
    {
        Dictionary<int, Koffie> koffies = (await _uow.KoffieRepo.GetAllAsync()).ToDictionary(x => x.Id);

        Bestelling bestelling = new Bestelling
        {
            KlantVoorNaam = vm.KlantVoorNaam,
            KlantAchterNaam = vm.KlantAchterNaam,
            KoffiesInBestelling = GewensteAantallen(vm.Regels)
                .Select(x => new BesteldeKoffie { KoffieId = x.Key, Koffie = koffies[x.Key], Aantal = x.Value })
                .ToList()
        };

        BestellingCreatedVm resultaat = await PlaatsBestellingAsync(bestelling);
        bestelling.Leverkosten = resultaat.Leverkosten;

        _uow.BestellingRepo.Add(bestelling);
        await _uow.SaveChangesAsync();

        resultaat.Id = bestelling.Id;
        return resultaat;
    }

    public async Task<bool> WerkBestellingBijAsync(int id, BestellingFormVm vm)
    {
        Bestelling bestelling = await _uow.BestellingRepo.GetBestellingWithDetailsAsync(id);

        if (bestelling == null)
        {
            return false;
        }

        Dictionary<int, Koffie> koffies = (await _uow.KoffieRepo.GetAllAsync()).ToDictionary(x => x.Id);
        Dictionary<int, int> gewenst = GewensteAantallen(vm.Regels);

        bestelling.KlantVoorNaam = vm.KlantVoorNaam;
        bestelling.KlantAchterNaam = vm.KlantAchterNaam;
        bestelling.Status = vm.Status;

        foreach (BesteldeKoffie regel in bestelling.KoffiesInBestelling.Where(x => !gewenst.ContainsKey(x.KoffieId)).ToList())
        {
            bestelling.KoffiesInBestelling.Remove(regel);
            _uow.BesteldeKoffiesRepo.Delete(regel);
        }

        foreach ((int koffieId, int aantal) in gewenst)
        {
            BesteldeKoffie? bestaand = bestelling.KoffiesInBestelling.FirstOrDefault(x => x.KoffieId == koffieId);

            if (bestaand == null)
            {
                bestelling.KoffiesInBestelling.Add(new BesteldeKoffie { KoffieId = koffieId, Koffie = koffies[koffieId], Aantal = aantal });
            }
            else
            {
                bestaand.Aantal = aantal;
            }
        }

        bestelling.Leverkosten = (await PlaatsBestellingAsync(bestelling)).Leverkosten;

        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerwijderBestellingAsync(int id)
    {
        Bestelling bestelling = await _uow.BestellingRepo.GetBestellingWithDetailsAsync(id);

        if (bestelling == null)
        {
            return false;
        }

        foreach (BesteldeKoffie regel in bestelling.KoffiesInBestelling.ToList())
        {
            _uow.BesteldeKoffiesRepo.Delete(regel);
        }

        _uow.BestellingRepo.Delete(bestelling);
        await _uow.SaveChangesAsync();
        return true;
    }

    private static Dictionary<int, int> GewensteAantallen(List<BestelRegelVm> regels)
    {
        return regels
            .Where(x => x.Aantal > 0)
            .GroupBy(x => x.KoffieId)
            .ToDictionary(x => x.Key, x => x.Sum(r => r.Aantal));
    }
}