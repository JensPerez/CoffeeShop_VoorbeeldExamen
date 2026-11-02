namespace CoffeeShop.Web.Data;

public interface IKoffieRepo : IGenericRepo<Koffie>
{
    public Task<Koffie> GetKoffieDetailAsync(int id);

    public Task<List<Bestelling>> GetBesteldeKoffiesAsync(int bestellingId);

    Task<List<Koffie>> GetByName(string zoekWaarde);

    Task<bool> IsBesteldAsync(int koffieId);
}