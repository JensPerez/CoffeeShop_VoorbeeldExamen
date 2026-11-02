namespace CoffeeShop.Web.Repositories;

public interface IGenericRepo<T> where T : class, IModel
{
    void Add(T model);

    void Update(T model);

    void Delete(T model);

    Task<T>? GetByIdAsync(int id);

    Task<List<T>> GetAllAsync();

    Task<bool> ExistsAsync(int id);

    Task<int[]> GetExistingIdsAsync();
}