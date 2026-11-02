namespace CoffeeShop.Web.ViewModels;

public class BestellingOverzichtVm
{
    public int Id { get; set; }
    public string KlantNaam { get; set; }
    public string Status { get; set; }
    public DateTime BestelDatum { get; set; }
    public decimal Leverkosten { get; set; }
    public decimal TotaalPrijs { get; set; }

    // De koffie's die in deze bestelling zitten
    public List<BesteldeKoffieVm> BesteldeKoffies { get; set; } = new();
}