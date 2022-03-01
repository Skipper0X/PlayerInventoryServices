using System.Linq.Expressions;

namespace Play.Common;

public interface IRepository<TEntity> where TEntity : IEntity
{
    Task<IReadOnlyCollection<TEntity>> GetAllAsync();
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression);
    Task<TEntity> GetAsync(Guid id);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task RemoveAsync(Guid id);
}