using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Web.Data;

public class IngredientRepo : GenericRepo<Ingredient>, IIngredientRepo
{
    public IngredientRepo(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<List<Ingredient>> GetIngredientenForKoffieAsync(int koffieId)
    {
        List<Ingredient?> ingredienten = await _context.IngredientKoffies
                        .Include(x => x.Ingredient)
                        .Where(x => x.KoffieId == koffieId)
                        .Select(x => x.Ingredient)
                        .ToListAsync();

        return ingredienten;
    }

    public async Task<List<Ingredient>> GetAllWithCategorieAsync()
    {
        List<Ingredient> ingredienten = await _context.Ingredienten
                        .Include(x => x.Categorie)
                        .OrderBy(x => x.CategorieId)
                        .ThenBy(x => x.Naam)
                        .ToListAsync();

        return ingredienten;
    }
}