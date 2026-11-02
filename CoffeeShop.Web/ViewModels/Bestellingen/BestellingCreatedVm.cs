namespace CoffeeShop.Web.ViewModels;

public class BestellingCreatedVm
{
    public int Id { get; set; }
    public string KlantNaam { get; set; } = string.Empty;
    public decimal Subtotaal { get; set; }
    public decimal Leverkosten { get; set; }
    public decimal Totaal { get; set; }
}