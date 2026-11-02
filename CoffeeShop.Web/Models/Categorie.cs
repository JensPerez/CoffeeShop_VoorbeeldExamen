namespace CoffeeShop.Web.Models;

public class Categorie : IModel
{
    public int Id { get; set; }

    public string Naam { get; set; }

    public List<Ingredient> Ingredienten { get; set; } = new();
}