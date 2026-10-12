using LawFirm.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Services;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _db;
    private readonly DbSet<T> _set;

    public Repository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public IQueryable<T> Query() => _set;

    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);

    public async Task AddAsync(T entity)
    {
        await _set.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public Task SaveAsync() => _db.SaveChangesAsync();

    public async Task DeleteAsync(int id)
    {
        var entity = await _set.FindAsync(id);
        if (entity is null) return;
        _set.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
