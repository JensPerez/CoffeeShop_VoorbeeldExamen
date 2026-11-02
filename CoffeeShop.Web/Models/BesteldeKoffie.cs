namespace CoffeeShop.Web.Models;

public class BesteldeKoffie : IModel
{
    public int Id { get; set; }

    public int BestellingId { get; set; }

    public int KoffieId { get; set; }

    public int Aantal { get; set; }

    public Bestelling? Bestelling { get; set; }

    public Koffie? Koffie { get; set; }
}