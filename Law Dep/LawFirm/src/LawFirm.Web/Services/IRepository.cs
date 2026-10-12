namespace LawFirm.Web.Services;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task SaveAsync();
    Task DeleteAsync(int id);
}
