using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Web.Data;

public class CoffeeShopDbContext : DbContext
{
    public CoffeeShopDbContext(DbContextOptions<CoffeeShopDbContext> options) : base(options)
    {
    }

    public DbSet<Bestelling> Bestellingen { get; set; }

    public DbSet<Categorie> Categorieen { get; set; }

    public DbSet<Koffie> Koffies { get; set; }

    public DbSet<Ingredient> Ingredienten { get; set; }

    public DbSet<IngredientKoffie> IngredientKoffies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        GenerateTables(modelBuilder);
        SeedData(modelBuilder);
    }

    private void GenerateTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bestelling>()
            .HasMany(x => x.KoffiesInBestelling)
            .WithOne(x => x.Bestelling)
            .HasForeignKey(x => x.BestellingId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<Categorie>()
            .HasMany(x => x.Ingredienten)
            .WithOne(x => x.Categorie)
            .HasForeignKey(x => x.CategorieId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<Koffie>()
            .HasMany(x => x.IngredientenInKoffie)
            .WithOne(x => x.Koffie)
            .HasForeignKey(x => x.KoffieId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<IngredientKoffie>()
            .HasOne(x => x.Ingredient)
            .WithMany(x => x.IngredientKoffies)
            .HasForeignKey(x => x.IngredientId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<IngredientKoffie>()
            .HasOne(x => x.Koffie)
            .WithMany(x => x.IngredientenInKoffie)
            .HasForeignKey(x => x.KoffieId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<BesteldeKoffie>()
            .HasOne(x => x.Bestelling)
            .WithMany(x => x.KoffiesInBestelling)
            .HasForeignKey(x => x.BestellingId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<BesteldeKoffie>()
            .HasOne(x => x.Koffie)
            .WithMany(x => x.BesteldeKoffies)
            .HasForeignKey(x => x.KoffieId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<Koffie>(entity =>
        {
            entity.Property(k => k.Naam)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(k => k.Prijs)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Categorie>(entity =>
        {
            entity.Property(c => c.Naam)
                  .IsRequired()
                  .HasMaxLength(100);
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(i => i.Naam)
                  .IsRequired()
                  .HasMaxLength(100);
        });
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // 1. Categorieën (5 stuks)
        modelBuilder.Entity<Categorie>().HasData(
            new Categorie { Id = 1, Naam = "Koffiebonen" },
            new Categorie { Id = 2, Naam = "Melk & Alternatieven" },
            new Categorie { Id = 3, Naam = "Siropen & Smaken" },
            new Categorie { Id = 4, Naam = "Toppings & Extra's" },
            new Categorie { Id = 5, Naam = "Thee & Kruiden" }
        );

        // 2. Ingrediënten (20 stuks verdeeld over de categorieën)
        modelBuilder.Entity<Ingredient>().HasData(
            // Categorie 1: Koffiebonen
            new Ingredient { Id = 1, CategorieId = 1, Naam = "Espressobonen", AantalInStock = 200, IsVegan = true },
            new Ingredient { Id = 2, CategorieId = 1, Naam = "Decafé Espressobonen", AantalInStock = 80, IsVegan = true },
            new Ingredient { Id = 3, CategorieId = 1, Naam = "Cold Brew Bonen", AantalInStock = 60, IsVegan = true },

            // Categorie 2: Melk & Alternatieven
            new Ingredient { Id = 4, CategorieId = 2, Naam = "Volle Melk", AantalInStock = 100, IsVegan = false },
            new Ingredient { Id = 5, CategorieId = 2, Naam = "Halfvolle Melk", AantalInStock = 80, IsVegan = false },
            new Ingredient { Id = 6, CategorieId = 2, Naam = "Havermelk", AantalInStock = 60, IsVegan = true },
            new Ingredient { Id = 7, CategorieId = 2, Naam = "Sojamelk", AantalInStock = 40, IsVegan = true },
            new Ingredient { Id = 8, CategorieId = 2, Naam = "Amandelmelk", AantalInStock = 30, IsVegan = true },

            // Categorie 3: Siropen & Smaken
            new Ingredient { Id = 9, CategorieId = 3, Naam = "Vanillesiroop", AantalInStock = 40, IsVegan = true },
            new Ingredient { Id = 10, CategorieId = 3, Naam = "Karamelsiroop", AantalInStock = 40, IsVegan = true },
            new Ingredient { Id = 11, CategorieId = 3, Naam = "Hazelnootsiroop", AantalInStock = 35, IsVegan = true },
            new Ingredient { Id = 12, CategorieId = 3, Naam = "Chocoladesaus", AantalInStock = 30, IsVegan = false },

            // Categorie 4: Toppings & Extra's
            new Ingredient { Id = 13, CategorieId = 4, Naam = "Slagroom", AantalInStock = 25, IsVegan = false },
            new Ingredient { Id = 14, CategorieId = 4, Naam = "Cacaopoeder", AantalInStock = 50, IsVegan = true },
            new Ingredient { Id = 15, CategorieId = 4, Naam = "Kaneel", AantalInStock = 50, IsVegan = true },
            new Ingredient { Id = 16, CategorieId = 4, Naam = "Karamelsaus", AantalInStock = 20, IsVegan = false },
            new Ingredient { Id = 17, CategorieId = 4, Naam = "IJsblokjes", AantalInStock = 200, IsVegan = true },
            new Ingredient { Id = 18, CategorieId = 4, Naam = "Vanille-ijs", AantalInStock = 15, IsVegan = false },

            // Categorie 5: Thee & Kruiden
            new Ingredient { Id = 19, CategorieId = 5, Naam = "Chai Kruidenmix", AantalInStock = 30, IsVegan = true },
            new Ingredient { Id = 20, CategorieId = 5, Naam = "Matchapoeder", AantalInStock = 25, IsVegan = true }
        );

        // 3. Koffies seeden (12 stuks)
        modelBuilder.Entity<Koffie>().HasData(
                    new Koffie { Id = 1, Naam = "Espresso", Prijs = 2.40m, Beschrijving = "Het hart van onze koffiebar. Een kort, krachtig shot van onze huisblend, vers gemalen en met precisie gezet, met een fluweelzachte crema en tonen van pure chocolade en geroosterde noten." },
                    new Koffie { Id = 2, Naam = "Ristretto", Prijs = 2.40m, Beschrijving = "Voor wie het nog intenser wil. Een extra korte extractie van onze espressobonen die alleen het zoetste en meest geconcentreerde deel van de koffie in je kopje laat belanden." },
                    new Koffie { Id = 3, Naam = "Americano", Prijs = 2.80m, Beschrijving = "Een dubbele espresso aangelengd met heet water. Licht van body maar vol van smaak, ideaal voor wie rustig wil genieten van het karakter van onze bonen." },
                    new Koffie { Id = 4, Naam = "Cappuccino", Prijs = 3.40m, Beschrijving = "De Italiaanse klassieker in perfecte balans: een derde espresso, een derde warme melk en een derde luchtig melkschuim, afgewerkt met een vleugje cacaopoeder." },
                    new Koffie { Id = 5, Naam = "Caffè Latte", Prijs = 3.60m, Beschrijving = "Zacht, romig en geruststellend. Een shot espresso met een royale hoeveelheid warme melk en een dun laagje fluweelzacht melkschuim." },
                    new Koffie { Id = 6, Naam = "Flat White", Prijs = 3.80m, Beschrijving = "Rechtstreeks uit Melbourne. Een dubbele ristretto met zijdezacht microschuim, waardoor de koffiesmaak sterker naar voren komt dan bij een latte." },
                    new Koffie { Id = 7, Naam = "Latte Macchiato", Prijs = 3.90m, Beschrijving = "Een feestje in een hoog glas. Warme halfvolle melk en melkschuim, 'gevlekt' met een shot espresso die prachtige laagjes vormt." },
                    new Koffie { Id = 8, Naam = "Cortado", Prijs = 3.20m, Beschrijving = "De Spaanse favoriet: espresso en warme melk in gelijke delen. Klein, krachtig en precies genoeg om de scherpe randjes van de espresso af te halen." },
                    new Koffie { Id = 9, Naam = "Caffè Mocha", Prijs = 4.20m, Beschrijving = "Voor de chocoladeliefhebbers. Espresso met rijke chocoladesaus en warme melk, bekroond met een toef verse slagroom." },
                    new Koffie { Id = 10, Naam = "Caramel Macchiato", Prijs = 4.40m, Beschrijving = "Zoet en verwennend. Warme melk met een vleugje vanillesiroop, een shot espresso erbovenop en afgewerkt met een rooster van karamelsaus." },
                    new Koffie { Id = 11, Naam = "Havermelk Latte", Prijs = 3.90m, Beschrijving = "Onze plantaardige favoriet. Espresso met romige, licht zoete havermelk die prachtig schuimt. Volledig vegan en minstens zo lekker." },
                    new Koffie { Id = 12, Naam = "Iced Cold Brew", Prijs = 4.00m, Beschrijving = "Achttien uur lang koud getrokken voor een zachte, ronde smaak met weinig bitterheid. Geserveerd over een glas vol ijsblokjes, perfect voor warme dagen." }
                );

        // 4. Koppeltabel Koffie <-> Ingredient (IngredientKoffie) seeden (met IngredientId's tussen 1 en 20)
        modelBuilder.Entity<IngredientKoffie>().HasData(
            // 1. Espresso (Espressobonen [1])
            new IngredientKoffie { Id = 1, KoffieId = 1, IngredientId = 1 },

            // 2. Ristretto (Espressobonen [1])
            new IngredientKoffie { Id = 2, KoffieId = 2, IngredientId = 1 },

            // 3. Americano (Espressobonen [1])
            new IngredientKoffie { Id = 3, KoffieId = 3, IngredientId = 1 },

            // 4. Cappuccino (Espressobonen [1], Volle Melk [4], Cacaopoeder [14])
            new IngredientKoffie { Id = 4, KoffieId = 4, IngredientId = 1 },
            new IngredientKoffie { Id = 5, KoffieId = 4, IngredientId = 4 },
            new IngredientKoffie { Id = 6, KoffieId = 4, IngredientId = 14 },

            // 5. Caffè Latte (Espressobonen [1], Volle Melk [4])
            new IngredientKoffie { Id = 7, KoffieId = 5, IngredientId = 1 },
            new IngredientKoffie { Id = 8, KoffieId = 5, IngredientId = 4 },

            // 6. Flat White (Espressobonen [1], Volle Melk [4])
            new IngredientKoffie { Id = 9, KoffieId = 6, IngredientId = 1 },
            new IngredientKoffie { Id = 10, KoffieId = 6, IngredientId = 4 },

            // 7. Latte Macchiato (Espressobonen [1], Halfvolle Melk [5])
            new IngredientKoffie { Id = 11, KoffieId = 7, IngredientId = 1 },
            new IngredientKoffie { Id = 12, KoffieId = 7, IngredientId = 5 },

            // 8. Cortado (Espressobonen [1], Halfvolle Melk [5])
            new IngredientKoffie { Id = 13, KoffieId = 8, IngredientId = 1 },
            new IngredientKoffie { Id = 14, KoffieId = 8, IngredientId = 5 },

            // 9. Caffè Mocha (Espressobonen [1], Volle Melk [4], Chocoladesaus [12], Slagroom [13])
            new IngredientKoffie { Id = 15, KoffieId = 9, IngredientId = 1 },
            new IngredientKoffie { Id = 16, KoffieId = 9, IngredientId = 4 },
            new IngredientKoffie { Id = 17, KoffieId = 9, IngredientId = 12 },
            new IngredientKoffie { Id = 18, KoffieId = 9, IngredientId = 13 },

            // 10. Caramel Macchiato (Espressobonen [1], Volle Melk [4], Vanillesiroop [9], Karamelsaus [16])
            new IngredientKoffie { Id = 19, KoffieId = 10, IngredientId = 1 },
            new IngredientKoffie { Id = 20, KoffieId = 10, IngredientId = 4 },
            new IngredientKoffie { Id = 21, KoffieId = 10, IngredientId = 9 },
            new IngredientKoffie { Id = 22, KoffieId = 10, IngredientId = 16 },

            // 11. Havermelk Latte (Espressobonen [1], Havermelk [6])
            new IngredientKoffie { Id = 23, KoffieId = 11, IngredientId = 1 },
            new IngredientKoffie { Id = 24, KoffieId = 11, IngredientId = 6 },

            // 12. Iced Cold Brew (Cold Brew Bonen [3], IJsblokjes [17])
            new IngredientKoffie { Id = 25, KoffieId = 12, IngredientId = 3 },
            new IngredientKoffie { Id = 26, KoffieId = 12, IngredientId = 17 }
        );

        // 5. Test Bestelling seeden (10 bestellingen)
        modelBuilder.Entity<Bestelling>().HasData(
            new Bestelling { Id = 1, BestelDatum = new DateTime(2026, 1, 1, 8, 15, 0, DateTimeKind.Utc), KlantVoorNaam = "Jan", KlantAchterNaam = "Peeters", Status = "In behandeling" },
            new Bestelling { Id = 2, BestelDatum = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Utc), KlantVoorNaam = "Els", KlantAchterNaam = "De Smet", Status = "Afgerond" },
            new Bestelling { Id = 3, BestelDatum = new DateTime(2026, 1, 3, 10, 15, 0, DateTimeKind.Utc), KlantVoorNaam = "Marc", KlantAchterNaam = "Verbeke", Status = "In behandeling" },
            new Bestelling { Id = 4, BestelDatum = new DateTime(2026, 1, 4, 11, 0, 0, DateTimeKind.Utc), KlantVoorNaam = "Sofie", KlantAchterNaam = "Claes", Status = "Geannuleerd" },
            new Bestelling { Id = 5, BestelDatum = new DateTime(2026, 1, 5, 12, 45, 0, DateTimeKind.Utc), KlantVoorNaam = "Bart", KlantAchterNaam = "Wouters", Status = "Afgerond" },
            new Bestelling { Id = 6, BestelDatum = new DateTime(2026, 1, 6, 14, 10, 0, DateTimeKind.Utc), KlantVoorNaam = "An", KlantAchterNaam = "Jacobs", Status = "In behandeling" },
            new Bestelling { Id = 7, BestelDatum = new DateTime(2026, 1, 7, 13, 20, 0, DateTimeKind.Utc), KlantVoorNaam = "Tom", KlantAchterNaam = "Goossens", Status = "Afgerond" },
            new Bestelling { Id = 8, BestelDatum = new DateTime(2026, 1, 8, 15, 50, 0, DateTimeKind.Utc), KlantVoorNaam = "Katrien", KlantAchterNaam = "Smets", Status = "In behandeling" },
            new Bestelling { Id = 9, BestelDatum = new DateTime(2026, 1, 9, 16, 30, 0, DateTimeKind.Utc), KlantVoorNaam = "Peter", KlantAchterNaam = "Willems", Status = "Afgerond" },
            new Bestelling { Id = 10, BestelDatum = new DateTime(2026, 1, 10, 17, 0, 0, DateTimeKind.Utc), KlantVoorNaam = "Hilde", KlantAchterNaam = "Maes", Status = "In behandeling" }
        );

        // 6. Bestelde koffies koppelen aan de bestelling
        modelBuilder.Entity<BesteldeKoffie>().HasData(
            new BesteldeKoffie { Id = 1, BestellingId = 1, KoffieId = 1, Aantal = 2 },
            new BesteldeKoffie { Id = 2, BestellingId = 1, KoffieId = 3, Aantal = 1 },
            new BesteldeKoffie { Id = 3, BestellingId = 2, KoffieId = 2, Aantal = 3 },
            new BesteldeKoffie { Id = 4, BestellingId = 2, KoffieId = 4, Aantal = 2 },
            new BesteldeKoffie { Id = 5, BestellingId = 2, KoffieId = 1, Aantal = 1 },
            new BesteldeKoffie { Id = 6, BestellingId = 3, KoffieId = 5, Aantal = 4 },
            new BesteldeKoffie { Id = 7, BestellingId = 4, KoffieId = 6, Aantal = 1 },
            new BesteldeKoffie { Id = 8, BestellingId = 5, KoffieId = 1, Aantal = 2 },
            new BesteldeKoffie { Id = 9, BestellingId = 5, KoffieId = 2, Aantal = 2 },
            new BesteldeKoffie { Id = 10, BestellingId = 5, KoffieId = 3, Aantal = 1 },
            new BesteldeKoffie { Id = 11, BestellingId = 5, KoffieId = 4, Aantal = 3 },
            new BesteldeKoffie { Id = 12, BestellingId = 6, KoffieId = 3, Aantal = 2 },
            new BesteldeKoffie { Id = 13, BestellingId = 7, KoffieId = 5, Aantal = 1 },
            new BesteldeKoffie { Id = 14, BestellingId = 7, KoffieId = 6, Aantal = 2 },
            new BesteldeKoffie { Id = 15, BestellingId = 8, KoffieId = 1, Aantal = 1 },
            new BesteldeKoffie { Id = 16, BestellingId = 8, KoffieId = 4, Aantal = 1 },
            new BesteldeKoffie { Id = 17, BestellingId = 8, KoffieId = 5, Aantal = 2 },
            new BesteldeKoffie { Id = 18, BestellingId = 9, KoffieId = 2, Aantal = 3 },
            new BesteldeKoffie { Id = 19, BestellingId = 10, KoffieId = 1, Aantal = 1 },
            new BesteldeKoffie { Id = 20, BestellingId = 10, KoffieId = 3, Aantal = 2 },
            new BesteldeKoffie { Id = 21, BestellingId = 10, KoffieId = 5, Aantal = 1 },
            new BesteldeKoffie { Id = 22, BestellingId = 10, KoffieId = 6, Aantal = 4 }
        );
    }
}
