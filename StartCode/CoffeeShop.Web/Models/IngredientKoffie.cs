namespace CoffeeShop.Web.Models;

public class IngredientKoffie : IModel
{
    public int Id { get; set; }

    public int KoffieId { get; set; }

    public int IngredientId { get; set; }

    public Koffie? Koffie { get; set; }

    public Ingredient? Ingredient { get; set; }
}