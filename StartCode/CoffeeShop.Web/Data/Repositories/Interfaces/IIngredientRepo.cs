namespace CoffeeShop.Web.Repositories;

public interface IIngredientRepo : IGenericRepo<Ingredient>
{
    // Alle ingrediënten die in de gegeven koffie zitten
    Task<List<Ingredient>> GetIngredientenForKoffieAsync(int koffieId);
}
