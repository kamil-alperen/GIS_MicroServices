using System.Linq.Expressions;
using MongoDB.Driver;

namespace GIS.Common.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;

        private readonly FilterDefinitionBuilder<T> filterDefinitionBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase mongoDatabase, string collectionName)
        {
            dbCollection = mongoDatabase.GetCollection<T>(collectionName);
        }

        public async Task<Result<IReadOnlyCollection<T>>> GetAllAsync()
        {
            try
            {
                return await dbCollection.Find(filterDefinitionBuilder.Empty).ToListAsync();
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyCollection<T>>.Failure(Error.FailureError("Database GetAll Failure", ex.Message));
            }
        }

        public async Task<Result<IReadOnlyCollection<T>>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await dbCollection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyCollection<T>>.Failure(Error.FailureError("Database GetAll Expression Failure", ex.Message));
            }
        }

        public async Task<Result<T>> GetAsync(Guid id)
        {
            try
            {
                FilterDefinition<T> filterDefinition = filterDefinitionBuilder.Eq<Guid>(ent => ent.Id, id);
                return await dbCollection.Find(filterDefinition).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(Error.FailureError("Database Get Failure", ex.Message));
            }
        }

        public async Task<Result<T>> GetAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await dbCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(Error.FailureError("Database Get Expression Failure", ex.Message));
            }
        }

        public async Task<Result<T>?> PostAsync(T entity)
        {
            try
            {
                FilterDefinition<T> filterDefinition = filterDefinitionBuilder.Eq<Guid>(ent => ent.Id, entity.Id);
                T record = await dbCollection.Find(filterDefinition).FirstOrDefaultAsync();
                if (entity == null || record != null)
                    return default(T);

                await dbCollection.InsertOneAsync(entity);

                return entity;
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(Error.FailureError("Database Create Failure", ex.Message));
            }
        }

        public async Task<Result<bool>> PutAsync(T countryEntity)
        {
            try
            {
                FilterDefinition<T> filterDefinition = filterDefinitionBuilder.Eq<Guid>(ent => ent.Id, countryEntity.Id);
                T record = await dbCollection.Find(filterDefinition).FirstOrDefaultAsync();
                if (countryEntity == null || record == null)
                    return false;

                dbCollection.ReplaceOne(filterDefinition, countryEntity);
                return true;
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(Error.FailureError("Database Update Failure", ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                FilterDefinition<T> filterDefinition = filterDefinitionBuilder.Eq<Guid>(ent => ent.Id, id);
                T removeCountry = await dbCollection.Find(filterDefinition).FirstOrDefaultAsync();
                if (removeCountry == null)
                    return false;

                dbCollection.DeleteOne(filterDefinition);

                return true;
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(Error.FailureError("Database Delete Failure", ex.Message));
            }
        }        
    }
}
