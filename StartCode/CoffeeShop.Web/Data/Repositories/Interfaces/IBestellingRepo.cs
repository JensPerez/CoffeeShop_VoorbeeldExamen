namespace CoffeeShop.Web.Repositories;

public interface IBestellingRepo : IGenericRepo<Bestelling>
{
    // Bestelling inclusief de bestelde koffies, of null als ze niet bestaat
    Task<Bestelling> GetBestellingWithDetailsAsync(int id);

    // Alle bestellingen inclusief de bestelde koffies, nieuwste eerst
    Task<List<Bestelling>> GetAllWithDetailsAsync();
}
