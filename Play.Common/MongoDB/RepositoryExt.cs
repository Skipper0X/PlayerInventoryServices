using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB;

public static class RepositoryExt
{
    public static IServiceCollection AddMongo(this IServiceCollection service)
    {
        // Set Bson Serializer...
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        service.AddSingleton(provider =>
        {
            // Get & Deserialize Settings From AppSettings...
            var configs = provider.GetService<IConfiguration>();
            var serviceSettings = configs.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = configs.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
            return mongoClient.GetDatabase(serviceSettings.ServiceName);
        });

        return service;
    }

    public static IServiceCollection AddMongoRepository<TEntity>(this IServiceCollection service, string collectionName)
        where TEntity : IEntity
    {
        service.AddSingleton<IRepository<TEntity>>(provider =>
        {
            var mongoDb = provider.GetService<IMongoDatabase>();
            return new MongoRepository<TEntity>(mongoDb, collectionName);
        });

        return service;
    }
}