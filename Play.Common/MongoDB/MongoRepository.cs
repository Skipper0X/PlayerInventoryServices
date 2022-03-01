using System.Linq.Expressions;
using MongoDB.Driver;

namespace Play.Common.MongoDB;

public class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
{
    private readonly IMongoCollection<TEntity> _itemCollection;
    private readonly FilterDefinitionBuilder<TEntity> _filterDefinitionBuilder = Builders<TEntity>.Filter;


    public MongoRepository(IMongoDatabase database, string name)
        => _itemCollection = database.GetCollection<TEntity>(name);

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
        => await _itemCollection.Find(_filterDefinitionBuilder.Empty).ToListAsync();

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression)
        => await _itemCollection.Find(expression).ToListAsync();

    public async Task<TEntity> GetAsync(Guid id)
    {
        var filter = _filterDefinitionBuilder.Eq(item => item.Id, id);
        return await _itemCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        => await _itemCollection.Find(expression).FirstOrDefaultAsync();

    public async Task CreateAsync(TEntity item)
    {
        if (item == null) throw new ArgumentNullException(nameof(TEntity));
        await _itemCollection.InsertOneAsync(item);
    }

    public async Task UpdateAsync(TEntity item)
    {
        if (item == null) throw new ArgumentNullException(nameof(TEntity));
        var filter = _filterDefinitionBuilder.Eq(existingItem => existingItem.Id, item.Id);
        await _itemCollection.ReplaceOneAsync(filter, item);
    }

    public async Task RemoveAsync(Guid id)
    {
        var filter = _filterDefinitionBuilder.Eq(item => item.Id, id);
        await _itemCollection.DeleteOneAsync(filter);
    }
}