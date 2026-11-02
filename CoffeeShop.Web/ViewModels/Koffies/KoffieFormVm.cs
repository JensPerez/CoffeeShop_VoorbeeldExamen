using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CoffeeShop.Web.ViewModels;

public class KoffieFormVm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Naam is verplicht.")]
    [StringLength(100, ErrorMessage = "Naam mag maximaal 100 tekens bevatten.")]
    public string Naam { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prijs is verplicht.")]
    [Range(0.01, 100, ErrorMessage = "Prijs moet tussen € 0,01 en € 100 liggen.")]
    public decimal Prijs { get; set; }

    [StringLength(1000, ErrorMessage = "Beschrijving mag maximaal 1000 tekens bevatten.")]
    public string? Beschrijving { get; set; }

    // Geselecteerde ingrediënten
    public List<int> IngredientIds { get; set; } = new();

    // Keuzelijst voor de view
    [ValidateNever]
    public List<IngredientVm> BeschikbareIngredienten { get; set; } = new();
}
