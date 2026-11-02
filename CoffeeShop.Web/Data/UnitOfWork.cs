using CoffeeShop.Web.Data.Repositories;

namespace CoffeeShop.Web.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CoffeeShopDbContext _context;

    // Todo: Specifieke repos
    private IKoffieRepo _koffieRepo;

    private IIngredientRepo _ingredientRepo;

    private IBestellingRepo _bestellingRepo;

    // Generieke Repos
    private IGenericRepo<Categorie>? _categorieRepo;

    private IGenericRepo<IngredientKoffie>? _ingredientKoffieRepo;

    private IGenericRepo<BesteldeKoffie>? _besteldeKoffiesRepo;

    public UnitOfWork(CoffeeShopDbContext context)
    {
        _context = context;
    }

    // Specifieke repos
    public IKoffieRepo KoffieRepo => _koffieRepo ??= new KoffieRepo(_context);

    public IIngredientRepo IngredientRepo => _ingredientRepo ??= new IngredientRepo(_context);

    public IBestellingRepo BestellingRepo => _bestellingRepo ??= new BestellingRepo(_context);

    //Generieke Repos
    public IGenericRepo<Categorie> CategorieRepo => _categorieRepo ??= new GenericRepo<Categorie>(_context);

    public IGenericRepo<IngredientKoffie> IngredientKoffieRepo => _ingredientKoffieRepo ??= new GenericRepo<IngredientKoffie>(_context);
    public IGenericRepo<BesteldeKoffie> BesteldeKoffiesRepo => _besteldeKoffiesRepo ??= new GenericRepo<BesteldeKoffie>(_context);

    // Slaat alle wijzigingen op binnen de context
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}