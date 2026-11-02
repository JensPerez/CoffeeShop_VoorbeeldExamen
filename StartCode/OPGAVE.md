# Examen – Café Crema

**Duur:** 140 minuten
**Startcode:** de solution `CoffeeShop.Web.slnx` (webproject + testproject)

Café Crema is een koffiebar met een online menukaart. Klanten kunnen de koffies bekijken (met ingrediënten
en of ze vegan zijn) en bestellingen plaatsen. Daarnaast is er een REST-API voor koffies en bestellingen.

Het datamodel, de database (met seed data), de ViewModels, de interfaces en de unit tests krijg je al.
Jij schrijft de **repositories**, de **services**, de **mappings**, de **controllers** en de **views**
van de menukaart.

Alles wat je nog moet uitwerken gooit een `NotImplementedException` of staat aangeduid met `TODO`.

---

## Voor je begint (± 5 min)

1. Open de solution en controleer dat alles compileert.
2. Start je SQL Server en maak de database aan:
   ```
   cd CoffeeShop.Web
   dotnet ef database update
   ```
   De connection string `SqlServerConnection` staat in `appsettings.json`.
3. Voer de tests uit. Ze compileren, maar bijna allemaal falen ze nog:
   ```
   dotnet test
   ```
   Naarmate je vordert, worden ze groen. **Pas de tests niet aan.**

---

## Het domein in het kort

| Entiteit          | Betekenis                                                               |
|-------------------|-------------------------------------------------------------------------|
| `Koffie`          | Een koffie op de menukaart (naam, prijs, beschrijving)                  |
| `Ingredient`      | Een ingrediënt met voorraad (`AantalInStock`), `IsVegan` en categorie   |
| `IngredientKoffie`| Koppeltabel: welke ingrediënten zitten in welke koffie                  |
| `Bestelling`      | Een bestelling van een klant, met status en leverkosten                 |
| `BesteldeKoffie`  | Eén regel in een bestelling: koffie + aantal                            |

### Bedrijfsregels

- **Vegan:** een koffie is vegan als **al** haar ingrediënten vegan zijn.
- **Leverkosten:** € 5,00. Vanaf een subtotaal van **€ 30,00 of meer** is de levering gratis.
- **Voorraad:** van elk ingrediënt van een bestelde koffie moet `AantalInStock` minstens gelijk zijn aan het
  bestelde aantal. Is dat niet zo, dan gooi je een `InvalidOperationException` met de boodschap
  `Onvoldoende voorraad voor ingrediënt '<naam>'` (je mag er extra uitleg achter zetten).
- **Afbeelding** van een koffie: `/img/{Naam}.png`.

---

## Stappenplan

Werk de stappen **in volgorde** af: elke stap bouwt verder op de vorige. De tijden zijn richtlijnen; hou ze
in de gaten. Loop je vast, ga dan verder met de volgende stap en kom er later op terug.

### Stap 1 – Repositories en Unit of Work (± 15 min · 15 punten)

Maak in `Data/Repositories` drie klassen die erven van `GenericRepo<T>` en de voorziene interface implementeren.
Gebruik de namespace `CoffeeShop.Web.Repositories`.

| Klasse           | Interface         | Methodes                                                                                       |
|------------------|-------------------|------------------------------------------------------------------------------------------------|
| `KoffieRepo`     | `IKoffieRepo`     | `GetKoffieDetailAsync` – koffie **inclusief ingrediënten én hun categorie**, of `null`         |
|                  |                   | `GetByName` – koffies waarvan de naam de zoekwaarde bevat                                      |
| `IngredientRepo` | `IIngredientRepo` | `GetIngredientenForKoffieAsync` – alle ingrediënten van één koffie                             |
| `BestellingRepo` | `IBestellingRepo` | `GetBestellingWithDetailsAsync` – bestelling **inclusief bestelde koffies en hun koffie**, of `null` |
|                  |                   | `GetAllWithDetailsAsync` – idem voor alle bestellingen, **nieuwste eerst**                     |

Maak de repositories daarna (lazy) aan in `UnitOfWork`, naar het voorbeeld van de generieke repositories.

> Voor deze stap zijn er geen unit tests. Je controleert hem in stap 5 en 6, wanneer je de app draait.

### Stap 2 – VeganService en mappings (± 10 min · 10 punten)

1. Werk `VeganService.BepaalVegan` uit volgens de bedrijfsregel.
2. Voeg in `Configuration/MapperProfile.cs` de ontbrekende Mapster-mappings toe:
   - `Koffie` → `KoffieListItemVm` (met `ImageUrl`)
   - `List<Koffie>` → `KoffieListVm` (de lijst komt in `Koffies`)
   - `Koffie` → `KoffieDetailVm` (met `ImageUrl`; `Ingredienten` komt uit `IngredientenInKoffie`)

**Tests:** `VeganServiceTests`, `MappingTests`

### Stap 3 – KoffieService (± 15 min · 15 punten)

| Methode                     | Gedrag                                                                                                  |
|-----------------------------|---------------------------------------------------------------------------------------------------------|
| `ValideerIngredientenAsync` | Lege lijst → foutmelding met *"minstens één"*. Onbestaand ingrediënt → foutmelding met *"bestaan niet"*. Anders `null`. |
| `MaakKoffieAsync`           | Maak de koffie aan uit het ViewModel, koppel elk ingrediënt **één keer** (dubbels negeren), voeg toe en sla op. Geef de koffie terug. |
| `WerkKoffieBijAsync`        | Haal de koffie op met details; `false` als ze niet bestaat. Werk de velden bij, **verwijder** koppelingen die niet meer geselecteerd zijn (via `IngredientKoffieRepo`) en **voeg** nieuwe toe. Sla op. |

> Tip: `vm.Adapt<Koffie>()` en `vm.Adapt(bestaandeKoffie)` gebruiken de mapping `KoffieFormVm → Koffie` die al voorzien is.

**Tests:** `KoffieServiceTests`

### Stap 4 – BestellingService (± 25 min · 20 punten)

| Methode                  | Gedrag                                                                                                      |
|--------------------------|-------------------------------------------------------------------------------------------------------------|
| `PlaatsBestellingAsync`  | Controleer **eerst** de voorraad (zie bedrijfsregel). Bereken daarna subtotaal, leverkosten en totaal en geef een `BestellingCreatedVm` terug (`KlantNaam` = voornaam + spatie + achternaam). Deze methode **slaat niets op**. |
| `BerekenTotaal`          | Som van de subtotalen van de bestelde koffies in het ViewModel (zonder leverkosten).                       |
| `ValideerRegelsAsync`    | Onbestaande koffie of negatief aantal → foutmelding met *"Ongeldige"*. Geen enkele regel met aantal > 0 → foutmelding met *"minstens één"*. Anders `null`. Controleer de ongeldige regels eerst. |
| `MaakBestellingAsync`    | Bouw een `Bestelling` op uit het formulier: regels met aantal 0 vallen weg, regels voor **dezelfde koffie worden samengevoegd**. Gebruik `PlaatsBestellingAsync` voor voorraad en leverkosten, bewaar de leverkosten in de bestelling, voeg toe en sla op. Geef het resultaat terug, met het nieuwe `Id`. |

> Tip: de `Koffie` van een `BesteldeKoffie` kan `null` zijn. Hou daar rekening mee bij het berekenen van het subtotaal.
>
> Tip: met `GroupBy` op `KoffieId` voeg je dubbele regels makkelijk samen.

**Tests:** `BestellingServiceTests`, `BestellingServiceCrudTests`

### Stap 5 – Menukaart in MVC (± 25 min · 20 punten)

1. **`KoffieController`** (maak de acties `async`):
   - `Index(zoekWaarde)`: alle koffies, of enkel de koffies die aan de zoekwaarde voldoen. Geef een
     `KoffieListVm` door aan de view.
   - `Details(id)`: haal de koffie op met details, geef `NotFound()` terug als ze niet bestaat, bepaal met de
     `VeganService` of ze vegan is en geef een `KoffieDetailVm` door aan de view.
2. **Views** `Koffie/Index.cshtml` en `Koffie/Details.cshtml`: vervang de hardgecodeerde data door het model.
   - Index: één kaart per koffie. Vind je niets, toon dan *"Geen koffies gevonden!"*. Laat de zoekterm in het
     zoekveld staan.
   - Details: naam, afbeelding, prijs, beschrijving, vegan-badge en de lijst ingrediënten met hun categorie.

Controleer in de browser: de menukaart toont 12 koffies, zoeken op *latte* geeft er 3, en de Cappuccino is
niet vegan, de Havermelk Latte wel.

### Stap 6 – API-controllers (± 20 min · 20 punten)

Werk de acties uit volgens de `TODO`-commentaar in de controllers. Let goed op het juiste resultaattype:

| Situatie                                    | Resultaat                                         |
|---------------------------------------------|---------------------------------------------------|
| Ongeldig id (`<= 0`) of id komt niet overeen | `BadRequest()` (zonder inhoud)                    |
| Validatiefout uit een service               | `BadRequest(...)` mét foutmelding                 |
| Onvoldoende voorraad                        | `BadRequest(ex.Message)`                          |
| Aangemaakt                                  | `CreatedAtAction(...)` met het nieuwe `id`         |
| Bijgewerkt                                  | `NoContent()`                                     |
| Bestaat niet                                | `NotFound()`                                      |

- **`KoffiesApiController`**: `GetKoffies`, `GetKoffie`, `CreateKoffie`, `UpdateKoffie`
- **`BestellingApiController`**: `GetBestellingById`, `GetAlleBestellingen`, `CreateBestelling`.
  Bij beide GET-acties vul je de `TotaalPrijs` in met `BerekenTotaal` van de service.

Laat de controllers het echte werk aan de services overlaten. Test de API met Scalar via `/scalar`.

**Tests:** `KoffiesApiControllerTests`, `BestellingApiControllerTests`

### Afronden (± 5 min)

- Draai `dotnet test` een laatste keer en kijk na of alles compileert. **Code die niet compileert, kan niet
  beoordeeld worden.** Zet onafgewerkte code in commentaar in plaats van ze te laten staan.
- Dien je oplossing in volgens de instructies van je lector.

---

## Beoordeling

| Stap | Onderdeel                         | Richttijd | Punten |
|-----:|-----------------------------------|----------:|-------:|
|      | Voor je begint                    |     5 min |        |
| 1    | Repositories en Unit of Work      |    15 min |     15 |
| 2    | VeganService en mappings          |    10 min |     10 |
| 3    | KoffieService                     |    15 min |     15 |
| 4    | BestellingService                 |    25 min |     20 |
| 5    | Menukaart in MVC                  |    25 min |     20 |
| 6    | API-controllers                   |    20 min |     20 |
|      | Afronden                          |     5 min |        |
|      | **Totaal**                        | **120 min** | **100** |

Je hebt 140 minuten: zo blijft er ± 20 minuten marge over om fouten op te sporen.

Groene tests zijn een goede indicatie, maar geen garantie: ook de kwaliteit van je code telt mee.
- Je gebruikt de Unit of Work en de repositories, nooit rechtstreeks de `DbContext` buiten de repositories.
- Bedrijfslogica staat in de services, niet in de controllers.
- Je gebruikt `async`/`await` correct.
- Je code is leesbaar, met duidelijke namen en zonder overbodige herhaling.
