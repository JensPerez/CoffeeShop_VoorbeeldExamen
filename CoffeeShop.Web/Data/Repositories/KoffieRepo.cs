using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Web.Data;

public class KoffieRepo : GenericRepo<Koffie>, IKoffieRepo
{
    public KoffieRepo(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<List<Bestelling>> GetBesteldeKoffiesAsync(int bestellingId)
    {
        List<Bestelling> koffies = await _context.Bestellingen
                        .Include(x => x.KoffiesInBestelling)
                        .ThenInclude(x => x.Koffie)
                        .ToListAsync();

        return koffies;
    }

    public async Task<List<Koffie>> GetByName(string zoekWaarde)
    {
        List<Koffie> koffies = await _context.Koffies
                            .Where(x => x.Naam.Contains(zoekWaarde))
                            .ToListAsync();

        return koffies;
    }

    public async Task<Koffie> GetKoffieDetailAsync(int id)
    {
        Koffie koffie = await _context.Koffies
                        .Include(x => x.IngredientenInKoffie)
                        .ThenInclude(x => x.Ingredient)
                        .ThenInclude(x => x.Categorie)
                        .FirstOrDefaultAsync(x => x.Id == id);

        return koffie;
    }

    public async Task<bool> IsBesteldAsync(int koffieId)
    {
        return await _context.Set<BesteldeKoffie>().AnyAsync(x => x.KoffieId == koffieId);
    }
}