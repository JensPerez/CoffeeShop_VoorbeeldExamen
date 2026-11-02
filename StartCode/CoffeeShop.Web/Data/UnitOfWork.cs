namespace CoffeeShop.Web.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CoffeeShopDbContext _context;

    // Generieke Repos
    private IGenericRepo<Categorie>? _categorieRepo;

    private IGenericRepo<IngredientKoffie>? _ingredientKoffieRepo;

    private IGenericRepo<BesteldeKoffie>? _besteldeKoffiesRepo;

    public UnitOfWork(CoffeeShopDbContext context)
    {
        _context = context;
    }

    // TODO: Specifieke repos
    //       Schrijf KoffieRepo, IngredientRepo en BestellingRepo en maak ze hier (lazy) aan,
    //       naar het voorbeeld van de generieke repos hieronder.
    public IKoffieRepo KoffieRepo => throw new NotImplementedException();

    public IIngredientRepo IngredientRepo => throw new NotImplementedException();

    public IBestellingRepo BestellingRepo => throw new NotImplementedException();

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
