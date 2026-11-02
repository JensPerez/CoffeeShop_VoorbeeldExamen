namespace CoffeeShop.Web.ViewModels;

public class KoffieListItemVm
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public decimal Prijs { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsVegan { get; set; }
}