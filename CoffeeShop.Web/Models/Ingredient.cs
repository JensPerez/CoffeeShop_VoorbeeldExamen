namespace CoffeeShop.Web.Models;

public class Ingredient : IModel
{
    public int Id { get; set; }

    public int CategorieId { get; set; }

    public string Naam { get; set; }

    public int AantalInStock { get; set; }

    public bool IsVegan { get; set; }

    public Categorie? Categorie { get; set; }

    public List<IngredientKoffie> IngredientKoffies { get; set; } = new();
}