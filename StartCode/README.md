# Café Crema – Startcode

De volledige opgave en het stappenplan vind je in [OPGAVE.md](OPGAVE.md).

## Wat is voorzien

- `Program.cs`, `CoffeeShopDbContext` (met seed data en migraties)
- `UnitOfWork` en de generieke repository (`GenericRepo<T>`)
- De interfaces van de specifieke repositories (`IKoffieRepo`, `IIngredientRepo`, `IBestellingRepo`)
- `MapperProfile` met enkele voorbeeldmappings
- De services (`KoffieService`, `BestellingService`, `VeganService`) met lege methodes
- De ViewModels
- De controllers met lege methodes
- De pagina's `Koffie/Index` en `Koffie/Details` met **hardgecodeerde** data
- De unit tests (`CoffeeShop.Web.Tests`)

Alles wat je nog moet uitwerken gooit een `NotImplementedException` of staat aangeduid met `TODO`.

## Opdracht

1. **Repositories** – Schrijf `KoffieRepo`, `IngredientRepo` en `BestellingRepo`. Ze erven van `GenericRepo<T>`
   en implementeren de voorziene interfaces. Gebruik de namespace `CoffeeShop.Web.Repositories`.
2. **Unit of Work** – Maak je repositories aan in `UnitOfWork`.
3. **Services** – Werk de lege methodes in de services uit.
4. **Mappings** – Voeg de mappings voor de menukaart en de detailpagina van een koffie toe aan `MapperProfile`.
5. **Controllers** – Werk de lege methodes in de controllers uit, en de acties `Index` en `Details` in `KoffieController`.
6. **Views** – Vervang de hardgecodeerde data in `Koffie/Index` en `Koffie/Details` door de data uit de database.

## Unit tests

De unit tests compileren en draaien vanaf het begin. In het begin falen ze bijna allemaal;
naarmate je de opdracht uitwerkt, worden ze groen. Zo kan je je vooruitgang opvolgen:

```
dotnet test
```
