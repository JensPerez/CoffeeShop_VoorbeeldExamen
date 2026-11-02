namespace CoffeeShop.Web.Models;

public class Bestelling : IModel
{
    public int Id { get; set; }
    public DateTime BestelDatum { get; set; } = DateTime.Now;
    public string KlantVoorNaam { get; set; } = string.Empty;
    public string KlantAchterNaam { get; set; } = string.Empty;
    public string Status { get; set; } = "In behandeling";
    public decimal Leverkosten { get; set; }
    public List<BesteldeKoffie> KoffiesInBestelling { get; set; } = new();
}