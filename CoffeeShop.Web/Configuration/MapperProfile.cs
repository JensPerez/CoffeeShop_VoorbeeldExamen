namespace CoffeeShop.Web.Configuration;

public class MapperProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Koffies
        config.NewConfig<List<Koffie>, KoffieListVm>()
            .Map(dest => dest.Koffies, src => src);

        config.NewConfig<Koffie, KoffieListItemVm>()
            .Map(dest => dest.Naam, src => src.Naam)
            .Map(dest => dest.ImageUrl, src => $"/img/{src.Naam}.png");

        config.NewConfig<Koffie, KoffieDetailVm>()
            .Map(dest => dest.Ingredienten, src => src.IngredientenInKoffie)
            .Map(dest => dest.ImageUrl, src => $"/img/{src.Naam}.png");

        config.NewConfig<Koffie, KoffieFormVm>()
            .Map(dest => dest.IngredientIds, src => src.IngredientenInKoffie.Select(x => x.IngredientId).ToList())
            .Ignore(dest => dest.BeschikbareIngredienten);

        config.NewConfig<KoffieFormVm, Koffie>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.IngredientenInKoffie)
            .Ignore(dest => dest.BesteldeKoffies);

        // Bestellingen
        config.NewConfig<Bestelling, BestellingOverzichtVm>()
            .Map(dest => dest.BesteldeKoffies, src => src.KoffiesInBestelling)
            .Map(dest => dest.KlantNaam, src => $"{src.KlantVoorNaam} {src.KlantAchterNaam}");

        config.NewConfig<BesteldeKoffie, BesteldeKoffieVm>()
            .Map(dest => dest.KoffieNaam, src => src.Koffie!.Naam)
            .Map(dest => dest.PrijsPerStuk, src => src.Koffie!.Prijs)
            .Map(dest => dest.Subtotaal, src => src.Koffie!.Prijs * src.Aantal);

        // Ingredienten
        config.NewConfig<IngredientKoffie, IngredientVm>()
            .Map(dest => dest.Id, src => src.IngredientId)
            .Map(dest => dest.AantalInStock, src => src.Ingredient!.AantalInStock)
            .Map(dest => dest.CategorieNaam, src => src.Ingredient!.Categorie!.Naam)
            .Map(dest => dest.IsVegan, src => src.Ingredient!.IsVegan)
            .Map(dest => dest.Naam, src => src.Ingredient!.Naam);
    }
}