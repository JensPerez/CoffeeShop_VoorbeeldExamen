namespace CoffeeShop.Web.Services;

public enum KoffieVerwijderResultaat
{
    Verwijderd,
    NietGevonden,
    Besteld
}

public interface IKoffieService
{
    // Geeft een foutmelding terug, of null als de ingrediënten geldig zijn
    Task<string?> ValideerIngredientenAsync(List<int> ingredientIds);

    Task<Koffie> MaakKoffieAsync(KoffieFormVm vm);

    // Geeft false terug als de koffie niet bestaat
    Task<bool> WerkKoffieBijAsync(int id, KoffieFormVm vm);

    Task<KoffieVerwijderResultaat> VerwijderKoffieAsync(int id);
}
