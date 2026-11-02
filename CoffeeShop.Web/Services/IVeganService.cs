namespace CoffeeShop.Web.Services
{
    public interface IVeganService
    {
        bool BepaalVegan(List<IngredientVm> ingredienten);
    }
}