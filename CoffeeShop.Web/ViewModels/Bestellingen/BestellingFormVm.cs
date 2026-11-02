using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Web.ViewModels;

public class BestellingFormVm
{
    public static readonly string[] Statussen = { "In behandeling", "Afgerond", "Geannuleerd" };

    public int Id { get; set; }

    [Required(ErrorMessage = "Voornaam is verplicht.")]
    [StringLength(100, ErrorMessage = "Voornaam mag maximaal 100 tekens bevatten.")]
    [Display(Name = "Voornaam")]
    public string KlantVoorNaam { get; set; } = string.Empty;

    [Required(ErrorMessage = "Achternaam is verplicht.")]
    [StringLength(100, ErrorMessage = "Achternaam mag maximaal 100 tekens bevatten.")]
    [Display(Name = "Achternaam")]
    public string KlantAchterNaam { get; set; } = string.Empty;

    public string Status { get; set; } = "In behandeling";

    // Eén regel per koffie op de menukaart; Aantal 0 = niet besteld
    public List<BestelRegelVm> Regels { get; set; } = new();
}
