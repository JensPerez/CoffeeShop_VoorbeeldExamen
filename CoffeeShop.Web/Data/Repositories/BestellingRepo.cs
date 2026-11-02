using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Web.Data.Repositories
{
    public class BestellingRepo : GenericRepo<Bestelling>, IBestellingRepo
    {
        public BestellingRepo(CoffeeShopDbContext context) : base(context)
        {
        }

        public async Task<Bestelling> GetBestellingWithDetailsAsync(int id)
        {
            return await _context.Bestellingen
                    .Include(x => x.KoffiesInBestelling)
                        .ThenInclude(x => x.Koffie)
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();
        }

        public async Task<List<Bestelling>> GetAllWithDetailsAsync()
        {
            return await _context.Bestellingen
                    .Include(x => x.KoffiesInBestelling)
                        .ThenInclude(x => x.Koffie)
                    .OrderByDescending(x => x.BestelDatum)
                    .ToListAsync();
        }
    }
}
