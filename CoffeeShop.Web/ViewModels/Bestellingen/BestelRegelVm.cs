using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CoffeeShop.Web.ViewModels;

public class BestelRegelVm
{
    public int KoffieId { get; set; }

    [ValidateNever]
    public string KoffieNaam { get; set; } = string.Empty;

    [ValidateNever]
    public decimal Prijs { get; set; }

    [Range(0, 50, ErrorMessage = "Aantal moet tussen 0 en 50 liggen.")]
    public int Aantal { get; set; }
}
