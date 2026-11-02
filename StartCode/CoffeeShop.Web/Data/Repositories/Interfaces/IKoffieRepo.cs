namespace CoffeeShop.Web.Repositories;

public interface IKoffieRepo : IGenericRepo<Koffie>
{
    // Koffie inclusief ingrediënten en hun categorie, of null als ze niet bestaat
    Task<Koffie> GetKoffieDetailAsync(int id);

    // Koffies waarvan de naam de zoekwaarde bevat
    Task<List<Koffie>> GetByName(string zoekWaarde);
}
