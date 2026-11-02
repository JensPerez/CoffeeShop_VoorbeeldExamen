namespace CoffeeShop.Web.ViewModels;

public class KoffieListVm
{
    public List<KoffieListItemVm> Koffies { get; set; } = new();

    public string? ZoekWaarde { get; set; }
}