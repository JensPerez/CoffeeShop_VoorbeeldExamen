namespace CoffeeShop.Web.Services;

public class KoffieService : IKoffieService
{
    private readonly IUnitOfWork _uow;

    public KoffieService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<string?> ValideerIngredientenAsync(List<int> ingredientIds)
    {
        if (!ingredientIds.Any())
        {
            return "Kies minstens één ingrediënt.";
        }

        int[] bestaandeIds = await _uow.IngredientRepo.GetExistingIdsAsync();

        return ingredientIds.Except(bestaandeIds).Any()
            ? "Een of meer gekozen ingrediënten bestaan niet."
            : null;
    }

    public async Task<Koffie> MaakKoffieAsync(KoffieFormVm vm)
    {
        Koffie koffie = vm.Adapt<Koffie>();
        koffie.IngredientenInKoffie = vm.IngredientIds
            .Distinct()
            .Select(ingredientId => new IngredientKoffie { IngredientId = ingredientId })
            .ToList();

        _uow.KoffieRepo.Add(koffie);
        await _uow.SaveChangesAsync();

        return koffie;
    }

    public async Task<bool> WerkKoffieBijAsync(int id, KoffieFormVm vm)
    {
        Koffie koffie = await _uow.KoffieRepo.GetKoffieDetailAsync(id);

        if (koffie == null)
        {
            return false;
        }

        vm.Adapt(koffie);

        // Ingrediënten synchroniseren: verwijderde koppelingen wissen, nieuwe toevoegen
        List<int> geselecteerd = vm.IngredientIds.Distinct().ToList();

        foreach (IngredientKoffie koppeling in koffie.IngredientenInKoffie.Where(x => !geselecteerd.Contains(x.IngredientId)).ToList())
        {
            _uow.IngredientKoffieRepo.Delete(koppeling);
        }

        foreach (int ingredientId in geselecteerd.Where(x => !koffie.IngredientenInKoffie.Any(k => k.IngredientId == x)))
        {
            koffie.IngredientenInKoffie.Add(new IngredientKoffie { IngredientId = ingredientId });
        }

        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<KoffieVerwijderResultaat> VerwijderKoffieAsync(int id)
    {
        Koffie koffie = await _uow.KoffieRepo.GetKoffieDetailAsync(id);

        if (koffie == null)
        {
            return KoffieVerwijderResultaat.NietGevonden;
        }

        if (await _uow.KoffieRepo.IsBesteldAsync(id))
        {
            return KoffieVerwijderResultaat.Besteld;
        }

        foreach (IngredientKoffie koppeling in koffie.IngredientenInKoffie.ToList())
        {
            _uow.IngredientKoffieRepo.Delete(koppeling);
        }

        _uow.KoffieRepo.Delete(koffie);
        await _uow.SaveChangesAsync();

        return KoffieVerwijderResultaat.Verwijderd;
    }
}
