namespace CoffeeShop.Web.Repositories;

public interface IUnitOfWork
{
    // Specifieke repos
    IKoffieRepo KoffieRepo { get; }
    IIngredientRepo IngredientRepo { get; }
    IBestellingRepo BestellingRepo { get; }

    // Generieke Repos
    IGenericRepo<Categorie> CategorieRepo { get; }
    IGenericRepo<IngredientKoffie> IngredientKoffieRepo { get; }
    IGenericRepo<BesteldeKoffie> BesteldeKoffiesRepo { get; }

    Task<int> SaveChangesAsync();
}
