namespace CoffeeShop.Web.Data.Repositories
{
    public interface IBestellingRepo: IGenericRepo<Bestelling>
    {
        Task<Bestelling> GetBestellingWithDetailsAsync(int id);

        Task<List<Bestelling>> GetAllWithDetailsAsync();
    }
}