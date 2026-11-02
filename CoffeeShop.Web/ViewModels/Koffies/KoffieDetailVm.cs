namespace CoffeeShop.Web.ViewModels;

public class KoffieDetailVm
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Beschrijving { get; set; } = string.Empty;
    public decimal Prijs { get; set; }
    public string ImageUrl { get; set; }
    public bool IsVegan { get; set; }

    // Lijst van gekoppelde ingrediënten
    public List<IngredientVm> Ingredienten { get; set; } = new();
}