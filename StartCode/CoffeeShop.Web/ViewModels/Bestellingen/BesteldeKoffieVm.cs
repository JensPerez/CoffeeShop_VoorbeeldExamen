namespace CoffeeShop.Web.ViewModels;

public class BesteldeKoffieVm
{
    public string KoffieNaam { get; set; } = string.Empty;
    public int Aantal { get; set; }
    public decimal PrijsPerStuk { get; set; }
    public decimal Subtotaal { get; set; } = 0m;
}