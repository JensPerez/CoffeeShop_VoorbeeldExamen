using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CoffeeShop.Web.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bestellingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BestelDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KlantVoorNaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KlantAchterNaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Leverkosten = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bestellingen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorieen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorieen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Koffies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prijs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Beschrijving = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Koffies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredienten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategorieId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AantalInStock = table.Column<int>(type: "int", nullable: false),
                    IsVegan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredienten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredienten_Categorieen_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "Categorieen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BesteldeKoffie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BestellingId = table.Column<int>(type: "int", nullable: false),
                    KoffieId = table.Column<int>(type: "int", nullable: false),
                    Aantal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BesteldeKoffie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BesteldeKoffie_Bestellingen_BestellingId",
                        column: x => x.BestellingId,
                        principalTable: "Bestellingen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BesteldeKoffie_Koffies_KoffieId",
                        column: x => x.KoffieId,
                        principalTable: "Koffies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngredientKoffies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KoffieId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientKoffies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientKoffies_Ingredienten_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredienten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientKoffies_Koffies_KoffieId",
                        column: x => x.KoffieId,
                        principalTable: "Koffies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Bestellingen",
                columns: new[] { "Id", "BestelDatum", "KlantAchterNaam", "KlantVoorNaam", "Leverkosten", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 8, 15, 0, 0, DateTimeKind.Utc), "Peeters", "Jan", 0m, "In behandeling" },
                    { 2, new DateTime(2026, 1, 2, 9, 30, 0, 0, DateTimeKind.Utc), "De Smet", "Els", 0m, "Afgerond" },
                    { 3, new DateTime(2026, 1, 3, 10, 15, 0, 0, DateTimeKind.Utc), "Verbeke", "Marc", 0m, "In behandeling" },
                    { 4, new DateTime(2026, 1, 4, 11, 0, 0, 0, DateTimeKind.Utc), "Claes", "Sofie", 0m, "Geannuleerd" },
                    { 5, new DateTime(2026, 1, 5, 12, 45, 0, 0, DateTimeKind.Utc), "Wouters", "Bart", 0m, "Afgerond" },
                    { 6, new DateTime(2026, 1, 6, 14, 10, 0, 0, DateTimeKind.Utc), "Jacobs", "An", 0m, "In behandeling" },
                    { 7, new DateTime(2026, 1, 7, 13, 20, 0, 0, DateTimeKind.Utc), "Goossens", "Tom", 0m, "Afgerond" },
                    { 8, new DateTime(2026, 1, 8, 15, 50, 0, 0, DateTimeKind.Utc), "Smets", "Katrien", 0m, "In behandeling" },
                    { 9, new DateTime(2026, 1, 9, 16, 30, 0, 0, DateTimeKind.Utc), "Willems", "Peter", 0m, "Afgerond" },
                    { 10, new DateTime(2026, 1, 10, 17, 0, 0, 0, DateTimeKind.Utc), "Maes", "Hilde", 0m, "In behandeling" }
                });

            migrationBuilder.InsertData(
                table: "Categorieen",
                columns: new[] { "Id", "Naam" },
                values: new object[,]
                {
                    { 1, "Koffiebonen" },
                    { 2, "Melk & Alternatieven" },
                    { 3, "Siropen & Smaken" },
                    { 4, "Toppings & Extra's" },
                    { 5, "Thee & Kruiden" }
                });

            migrationBuilder.InsertData(
                table: "Koffies",
                columns: new[] { "Id", "Beschrijving", "Naam", "Prijs" },
                values: new object[,]
                {
                    { 1, "Het hart van onze koffiebar. Een kort, krachtig shot van onze huisblend, vers gemalen en met precisie gezet, met een fluweelzachte crema en tonen van pure chocolade en geroosterde noten.", "Espresso", 2.40m },
                    { 2, "Voor wie het nog intenser wil. Een extra korte extractie van onze espressobonen die alleen het zoetste en meest geconcentreerde deel van de koffie in je kopje laat belanden.", "Ristretto", 2.40m },
                    { 3, "Een dubbele espresso aangelengd met heet water. Licht van body maar vol van smaak, ideaal voor wie rustig wil genieten van het karakter van onze bonen.", "Americano", 2.80m },
                    { 4, "De Italiaanse klassieker in perfecte balans: een derde espresso, een derde warme melk en een derde luchtig melkschuim, afgewerkt met een vleugje cacaopoeder.", "Cappuccino", 3.40m },
                    { 5, "Zacht, romig en geruststellend. Een shot espresso met een royale hoeveelheid warme melk en een dun laagje fluweelzacht melkschuim.", "Caffè Latte", 3.60m },
                    { 6, "Rechtstreeks uit Melbourne. Een dubbele ristretto met zijdezacht microschuim, waardoor de koffiesmaak sterker naar voren komt dan bij een latte.", "Flat White", 3.80m },
                    { 7, "Een feestje in een hoog glas. Warme halfvolle melk en melkschuim, 'gevlekt' met een shot espresso die prachtige laagjes vormt.", "Latte Macchiato", 3.90m },
                    { 8, "De Spaanse favoriet: espresso en warme melk in gelijke delen. Klein, krachtig en precies genoeg om de scherpe randjes van de espresso af te halen.", "Cortado", 3.20m },
                    { 9, "Voor de chocoladeliefhebbers. Espresso met rijke chocoladesaus en warme melk, bekroond met een toef verse slagroom.", "Caffè Mocha", 4.20m },
                    { 10, "Zoet en verwennend. Warme melk met een vleugje vanillesiroop, een shot espresso erbovenop en afgewerkt met een rooster van karamelsaus.", "Caramel Macchiato", 4.40m },
                    { 11, "Onze plantaardige favoriet. Espresso met romige, licht zoete havermelk die prachtig schuimt. Volledig vegan en minstens zo lekker.", "Havermelk Latte", 3.90m },
                    { 12, "Achttien uur lang koud getrokken voor een zachte, ronde smaak met weinig bitterheid. Geserveerd over een glas vol ijsblokjes, perfect voor warme dagen.", "Iced Cold Brew", 4.00m }
                });

            migrationBuilder.InsertData(
                table: "BesteldeKoffie",
                columns: new[] { "Id", "Aantal", "BestellingId", "KoffieId" },
                values: new object[,]
                {
                    { 1, 2, 1, 1 },
                    { 2, 1, 1, 3 },
                    { 3, 3, 2, 2 },
                    { 4, 2, 2, 4 },
                    { 5, 1, 2, 1 },
                    { 6, 4, 3, 5 },
                    { 7, 1, 4, 6 },
                    { 8, 2, 5, 1 },
                    { 9, 2, 5, 2 },
                    { 10, 1, 5, 3 },
                    { 11, 3, 5, 4 },
                    { 12, 2, 6, 3 },
                    { 13, 1, 7, 5 },
                    { 14, 2, 7, 6 },
                    { 15, 1, 8, 1 },
                    { 16, 1, 8, 4 },
                    { 17, 2, 8, 5 },
                    { 18, 3, 9, 2 },
                    { 19, 1, 10, 1 },
                    { 20, 2, 10, 3 },
                    { 21, 1, 10, 5 },
                    { 22, 4, 10, 6 }
                });

            migrationBuilder.InsertData(
                table: "Ingredienten",
                columns: new[] { "Id", "AantalInStock", "CategorieId", "IsVegan", "Naam" },
                values: new object[,]
                {
                    { 1, 200, 1, true, "Espressobonen" },
                    { 2, 80, 1, true, "Decafé Espressobonen" },
                    { 3, 60, 1, true, "Cold Brew Bonen" },
                    { 4, 100, 2, false, "Volle Melk" },
                    { 5, 80, 2, false, "Halfvolle Melk" },
                    { 6, 60, 2, true, "Havermelk" },
                    { 7, 40, 2, true, "Sojamelk" },
                    { 8, 30, 2, true, "Amandelmelk" },
                    { 9, 40, 3, true, "Vanillesiroop" },
                    { 10, 40, 3, true, "Karamelsiroop" },
                    { 11, 35, 3, true, "Hazelnootsiroop" },
                    { 12, 30, 3, false, "Chocoladesaus" },
                    { 13, 25, 4, false, "Slagroom" },
                    { 14, 50, 4, true, "Cacaopoeder" },
                    { 15, 50, 4, true, "Kaneel" },
                    { 16, 20, 4, false, "Karamelsaus" },
                    { 17, 200, 4, true, "IJsblokjes" },
                    { 18, 15, 4, false, "Vanille-ijs" },
                    { 19, 30, 5, true, "Chai Kruidenmix" },
                    { 20, 25, 5, true, "Matchapoeder" }
                });

            migrationBuilder.InsertData(
                table: "IngredientKoffies",
                columns: new[] { "Id", "IngredientId", "KoffieId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 2 },
                    { 3, 1, 3 },
                    { 4, 1, 4 },
                    { 5, 4, 4 },
                    { 6, 14, 4 },
                    { 7, 1, 5 },
                    { 8, 4, 5 },
                    { 9, 1, 6 },
                    { 10, 4, 6 },
                    { 11, 1, 7 },
                    { 12, 5, 7 },
                    { 13, 1, 8 },
                    { 14, 5, 8 },
                    { 15, 1, 9 },
                    { 16, 4, 9 },
                    { 17, 12, 9 },
                    { 18, 13, 9 },
                    { 19, 1, 10 },
                    { 20, 4, 10 },
                    { 21, 9, 10 },
                    { 22, 16, 10 },
                    { 23, 1, 11 },
                    { 24, 6, 11 },
                    { 25, 3, 12 },
                    { 26, 17, 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BesteldeKoffie_BestellingId",
                table: "BesteldeKoffie",
                column: "BestellingId");

            migrationBuilder.CreateIndex(
                name: "IX_BesteldeKoffie_KoffieId",
                table: "BesteldeKoffie",
                column: "KoffieId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredienten_CategorieId",
                table: "Ingredienten",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientKoffies_IngredientId",
                table: "IngredientKoffies",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientKoffies_KoffieId",
                table: "IngredientKoffies",
                column: "KoffieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BesteldeKoffie");

            migrationBuilder.DropTable(
                name: "IngredientKoffies");

            migrationBuilder.DropTable(
                name: "Bestellingen");

            migrationBuilder.DropTable(
                name: "Ingredienten");

            migrationBuilder.DropTable(
                name: "Koffies");

            migrationBuilder.DropTable(
                name: "Categorieen");
        }
    }
}
