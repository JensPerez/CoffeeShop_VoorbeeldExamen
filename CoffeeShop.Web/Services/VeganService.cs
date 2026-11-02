namespace CoffeeShop.Web.Services
{
    public class VeganService : IVeganService
    {
        public bool BepaalVegan(List<IngredientVm> ingredienten)
        {
            return ingredienten.All(x => x.IsVegan == true);
        }
    }
}
