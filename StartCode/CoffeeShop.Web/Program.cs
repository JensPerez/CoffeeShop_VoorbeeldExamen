using Mapster;
using Microsoft.EntityFrameworkCore;
using CoffeeShop.Web.Services;
using Scalar.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi(); // .NET 9 ingebouwde OpenAPI

// Add Mapster configuration
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

// Set database connection
builder.Services.AddDbContext<CoffeeShopDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

// Add services to the container.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBestellingService, BestellingService>();
builder.Services.AddScoped<IVeganService, VeganService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 1. Genereer het OpenAPI JSON-bestand in Development (/openapi/v1.json)
    app.MapOpenApi();

    // 2. Koppel Scalar hier direct aan (zonder afwijkend route-patroon)
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Café Crema API")
            .WithTheme(ScalarTheme.Mars);
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStatusCodePagesWithReExecute("/Home/PageNotFound");

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Zorg dat API en MVC controllers gelijktijdig worden geregistreerd
app.MapControllers();

app.Run();