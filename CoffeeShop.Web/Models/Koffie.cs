namespace CoffeeShop.Web.Models;

public class Koffie : IModel
{
    public int Id { get; set; }

    public string Naam { get; set; }

    public decimal Prijs { get; set; }

    public string? Beschrijving { get; set; }

    public bool IsVegan { get; }

    public List<IngredientKoffie> IngredientenInKoffie { get; set; } = new();

    public List<BesteldeKoffie> BesteldeKoffies { get; set; } = new();
}