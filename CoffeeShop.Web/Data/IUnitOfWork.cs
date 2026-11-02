using CoffeeShop.Web.Data.Repositories;

namespace CoffeeShop.Web.Repositories;

public interface IUnitOfWork
{
    IKoffieRepo KoffieRepo { get; }
    IIngredientRepo IngredientRepo { get; }
    IBestellingRepo BestellingRepo { get; }
    IGenericRepo<Categorie> CategorieRepo { get; }
    IGenericRepo<IngredientKoffie> IngredientKoffieRepo { get; }
    IGenericRepo<BesteldeKoffie> BesteldeKoffiesRepo { get; }

    Task<int> SaveChangesAsync();
}