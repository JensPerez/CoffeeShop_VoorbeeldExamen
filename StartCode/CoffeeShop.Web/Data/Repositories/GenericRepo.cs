using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Web.Repositories;

public class GenericRepo<T> : IGenericRepo<T> where T : class, IModel
{
    protected CoffeeShopDbContext _context;

    public GenericRepo(CoffeeShopDbContext context)
    {
        _context = context;
    }

    public void Add(T model)
    {
        _context.Set<T>().Add(model);
    }

    public void Delete(T model)
    {
        _context.Set<T>().Remove(model);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Set<T>().AnyAsync(x => x.Id == id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        // Set<TEntity> pakt automatisch de juiste tabel (bv. Laptops of Producten),
        // gebaseerd op het type dat T zal vervangen.
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T>? GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<int[]> GetExistingIdsAsync()
    {
        return await _context.Set<T>().Select(x => x.Id).ToArrayAsync();
    }

    public void Update(T model)
    {
        _context.Set<T>().Attach(model);
        _context.Entry(model).State = EntityState.Modified;
    }
}