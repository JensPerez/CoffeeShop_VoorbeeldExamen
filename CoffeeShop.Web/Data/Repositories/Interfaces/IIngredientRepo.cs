namespace CoffeeShop.Web.Data;

public interface IIngredientRepo : IGenericRepo<Ingredient>
{
    public Task<List<Ingredient>> GetIngredientenForKoffieAsync(int koffieId);

    public Task<List<Ingredient>> GetAllWithCategorieAsync();
}