namespace CoffeeShop.Web.ViewModels;

public class IngredientVm
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public bool IsVegan { get; set; }
    public int AantalInStock { get; set; }
    public string CategorieNaam { get; set; } = string.Empty;
}