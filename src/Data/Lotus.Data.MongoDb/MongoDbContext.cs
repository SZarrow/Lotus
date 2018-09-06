using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Lotus.Core;
using Lotus.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Lotus.Data.MongoDb
{
    public class MongoDbContext : IDbContext
    {
        private static readonly ILogger s_logger = LogManager.GetLogger();
        private readonly IMongoDatabase _db;
        private readonly Collection<KeyValuePair<OperationType, Object>> _uowObjects;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public MongoDbContext(MongoDbOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            var client = new MongoClient(option.ConnectionString);
            _db = client.GetDatabase(option.DbName);
            _uowObjects = new Collection<KeyValuePair<OperationType, Object>>();
        }

        protected MongoDbContext() { }

        public XResult<Boolean> Add<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(entity)));
            }

            var collection = GetCollection<TEntity>();
            if (collection == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(collection)));
            }

            _uowObjects.Add(new KeyValuePair<OperationType, Object>(OperationType.Add, entity));
            return new XResult<Boolean>(true);
        }

        public XResult<Boolean> AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities == null || entities.Count() == 0)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(entities)));
            }

            var collection = GetCollection<TEntity>();
            if (collection == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(collection)));
            }

            _uowObjects.Add(new KeyValuePair<OperationType, Object>(OperationType.Add, entities));
            return new XResult<Boolean>(true);
        }

        public IQueryable<TEntity> AsQueryable<TEntity>()
        {
            var collection = GetCollection<TEntity>();
            return collection != null ? collection.AsQueryable() : null;
        }

        public IEnumerable<TEntity> Find<TEntity>(Expression<Func<TEntity, Boolean>> filter) where TEntity : class
        {
            var collection = GetCollection<TEntity>();
            if (collection == null)
            {
                return null;
            }

            try
            {
                return collection.Find(filter).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, Boolean>> filter, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class
        {
            var collection = GetCollection<TEntity>();
            if (collection == null)
            {
                return null;
            }

            try
            {
                var entities = await collection.FindAsync(filter);
                return entities.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public XResult<Boolean> Remove<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(entity)));
            }

            var collection = GetCollection<TEntity>();
            if (collection == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(collection)));
            }

            _uowObjects.Add(new KeyValuePair<OperationType, Object>(OperationType.Delete, entity));
            return new XResult<Boolean>(true);
        }

        public XResult<Boolean> RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities == null || entities.Count() == 0)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(entities)));
            }

            var collection = GetCollection<TEntity>();
            if (collection == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(collection)));
            }

            _uowObjects.Add(new KeyValuePair<OperationType, Object>(OperationType.Delete, entities));
            return new XResult<Boolean>(true);
        }

        public XResult<Boolean> Update<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(entity)));
            }

            var collection = GetCollection<TEntity>();
            if (collection == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(collection)));
            }

            _uowObjects.Add(new KeyValuePair<OperationType, Object>(OperationType.Update, entity));
            return new XResult<Boolean>(true);
        }

        public XResult<Boolean> UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities == null || entities.Count() == 0)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(entities)));
            }

            var collection = GetCollection<TEntity>();
            if (collection == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(collection)));
            }

            _uowObjects.Add(new KeyValuePair<OperationType, Object>(OperationType.Update, entities));
            return new XResult<Boolean>(true);
        }

        public XResult<Int32> SaveChange(Boolean useTransaction = false)
        {
            Int32 successCount = 0;

            if (_uowObjects.Count == 0)
            {
                return new XResult<Int32>(0);
            }

            IClientSessionHandle tx = null;
            try
            {
                if (useTransaction)
                {
                    tx = _db.Client.StartSession();
                    tx.StartTransaction();
                }

                foreach (var uo in _uowObjects)
                {
                    var result = Persist(uo, tx);
                    if (result.Success)
                    {
                        successCount += result.Value;
                    }
                    else
                    {
                        s_logger.Error(result.Exceptions[0], $"Operation:{uo.Key.ToString()}, Entity: {uo.Value.ToJson()}");
                    }
                }

                if (tx != null)
                {
                    if (successCount == _uowObjects.Count)
                    {
                        tx.CommitTransaction();
                    }
                }

                _uowObjects.Clear();
            }
            finally
            {
                if (tx != null)
                {
                    tx.Dispose();
                }
            }

            return new XResult<Int32>(successCount);
        }

        public async Task<XResult<Int32>> SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken), Boolean useTransaction = false)
        {
            List<KeyValuePair<OperationType, Object>> uowObjects = new List<KeyValuePair<OperationType, Object>>(0);

            try
            {
                _lock.EnterReadLock();
                uowObjects = _uowObjects.ToList();
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                {
                    _lock.ExitReadLock();
                }
            }

            if (uowObjects.Count == 0)
            {
                return new XResult<Int32>(0);
            }

            IClientSessionHandle tx = null;
            try
            {
                if (useTransaction)
                {
                    tx = _db.Client.StartSession();
                    tx.StartTransaction();
                }

                List<Task<XResult<Int32>>> tasks = new List<Task<XResult<Int32>>>(uowObjects.Count);
                foreach (var uo in uowObjects)
                {
                    tasks.Add(PersistAsync(uo, cancellationToken, tx));
                }

                Int32 successCount = 0;

                foreach (var task in tasks)
                {
                    var completedTask = await Task.WhenAny(task, Task.Delay(10 * 1000));
                    if (completedTask == task)
                    {
                        var persistResult = task.Result;
                        if (persistResult.Success)
                        {
                            successCount += persistResult.Value;
                        }
                        else
                        {
                            s_logger.Error(persistResult.Exceptions[0]);
                        }
                    }
                }


                if (tx != null)
                {
                    if (successCount == uowObjects.Count)
                    {
                        tx.CommitTransaction();
                    }
                }

                try
                {
                    _lock.EnterWriteLock();
                    foreach (var item in uowObjects)
                    {
                        _uowObjects.Remove(item);
                    }
                }
                finally
                {
                    if (_lock.IsWriteLockHeld)
                    {
                        _lock.ExitWriteLock();
                    }
                }

                return new XResult<Int32>(successCount);
            }
            finally
            {
                if (tx != null)
                {
                    tx.Dispose();
                }
            }
        }

        public void Dispose() { }

        private IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            String typeName = typeof(TEntity).Name;
            try
            {
                var collection = _db.GetCollection<TEntity>(typeName);
                if (collection == null)
                {
                    collection = _db.GetCollection<TEntity>(typeName.ToLower());
                }

                return collection;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return null;
            }
        }

        private IMongoCollection<Object> GetCollection(Type entityType)
        {
            try
            {
                var collection = _db.GetCollection<Object>(entityType.Name);
                if (collection == null)
                {
                    collection = _db.GetCollection<Object>(entityType.Name.ToLower());
                }

                return collection;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return null;
            }
        }

        private XResult<Int32> Persist(KeyValuePair<OperationType, Object> uo, IClientSessionHandle tx = null)
        {
            switch (uo.Key)
            {
                case OperationType.Add:
                    return PersistAdd(uo.Value, tx);
                case OperationType.Update:
                    return PersistUpdate(uo.Value, tx);
                case OperationType.Delete:
                    return PersistDelete(uo.Value, tx);
            }

            return new XResult<Int32>(0, new NotSupportedException(uo.Key.ToString()));
        }

        private async Task<XResult<Int32>> PersistAsync(KeyValuePair<OperationType, Object> uo, CancellationToken cancellationToken = default(CancellationToken), IClientSessionHandle tx = null)
        {
            switch (uo.Key)
            {
                case OperationType.Add:
                    return await PersistAddAsync(uo.Value, cancellationToken, tx);
                case OperationType.Update:
                    return await PersistUpdateAsync(uo.Value, cancellationToken, tx);
                case OperationType.Delete:
                    return await PersistDeleteAsync(uo.Value, cancellationToken, tx);
            }

            return new XResult<Int32>(0, new NotSupportedException(uo.Key.ToString()));
        }

        private XResult<Int32> PersistAdd(Object value, IClientSessionHandle tx = null)
        {
            var entityType = GetEntityType(value);
            if (entityType == null)
            {
                return new XResult<Int32>(0, new ArgumentNullException(nameof(entityType)));
            }

            var collection = this.GetCollection(entityType);
            if (collection == null)
            {
                return new XResult<Int32>(0, new NullReferenceException(nameof(collection)));
            }

            try
            {
                Int32 addedCount = 0;

                if (value is IEnumerable)
                {
                    if (tx != null)
                    {
                        collection.InsertMany(tx, value as IEnumerable<Object>);
                    }
                    else
                    {
                        collection.InsertMany(value as IEnumerable<Object>);
                    }

                    addedCount = (value as IEnumerable<Object>).Count();
                }
                else
                {
                    if (tx != null)
                    {
                        collection.InsertOne(tx, value);
                    }
                    else
                    {
                        collection.InsertOne(value);
                    }

                    addedCount = 1;
                }

                return new XResult<Int32>(addedCount);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return new XResult<Int32>(0, ex);
            }
        }

        private async Task<XResult<Int32>> PersistAddAsync(Object value, CancellationToken cancellationToken = default(CancellationToken), IClientSessionHandle tx = null)
        {
            var entityType = GetEntityType(value);
            if (entityType == null)
            {
                return new XResult<Int32>(0, new ArgumentNullException(nameof(entityType)));
            }

            var collection = this.GetCollection(entityType);
            if (collection == null)
            {
                return new XResult<Int32>(0, new NullReferenceException(nameof(collection)));
            }

            try
            {
                Int32 addedCount = 0;

                if (value is IEnumerable)
                {
                    if (tx != null)
                    {
                        await collection.InsertManyAsync(tx, value as IEnumerable<Object>, null, cancellationToken);
                    }
                    else
                    {
                        await collection.InsertManyAsync(value as IEnumerable<Object>, null, cancellationToken);
                    }

                    addedCount = (value as IEnumerable<Object>).Count();
                }
                else
                {
                    if (tx != null)
                    {
                        await collection.InsertOneAsync(tx, value, null, cancellationToken);
                    }
                    else
                    {
                        await collection.InsertOneAsync(value, null, cancellationToken);
                    }

                    addedCount = 1;
                }

                return new XResult<Int32>(addedCount);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return new XResult<Int32>(0, ex);
            }
        }

        private XResult<Int32> PersistUpdate(Object value, IClientSessionHandle tx = null)
        {
            var entityType = GetEntityType(value);
            if (entityType == null)
            {
                return new XResult<Int32>(0, new ArgumentNullException(nameof(entityType)));
            }

            var collection = this.GetCollection(entityType);
            if (collection == null)
            {
                return new XResult<Int32>(0, new NullReferenceException(nameof(collection)));
            }

            try
            {
                ReplaceOneResult updateResult = null;
                Int64 updatedCount = 0;

                if (value is IEnumerable<IdentityEntity>)
                {
                    foreach (var entity in value as IEnumerable<IdentityEntity>)
                    {
                        updateResult = tx != null ?
                            collection.ReplaceOne(tx, x => (x as IdentityEntity).MongoId == entity.MongoId, entity, new UpdateOptions() { IsUpsert = false }) :
                            collection.ReplaceOne(x => (x as IdentityEntity).MongoId == entity.MongoId, entity, new UpdateOptions() { IsUpsert = false });
                        updatedCount += updateResult.ModifiedCount;
                    }
                }
                else
                {
                    updateResult = tx != null ?
                        collection.ReplaceOne(tx, x => (x as IdentityEntity).MongoId == (value as IdentityEntity).MongoId, value, new UpdateOptions() { IsUpsert = false }) :
                        collection.ReplaceOne(x => (x as IdentityEntity).MongoId == (value as IdentityEntity).MongoId, value, new UpdateOptions() { IsUpsert = false });
                    updatedCount = updateResult.ModifiedCount;
                }

                return new XResult<Int32>(Convert.ToInt32(updatedCount));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return new XResult<Int32>(0, ex);
            }
        }

        private async Task<XResult<Int32>> PersistUpdateAsync(Object value, CancellationToken cancellationToken = default(CancellationToken), IClientSessionHandle tx = null)
        {
            var entityType = GetEntityType(value);
            if (entityType == null)
            {
                return new XResult<Int32>(0, new ArgumentNullException(nameof(entityType)));
            }

            var collection = this.GetCollection(entityType);
            if (collection == null)
            {
                return new XResult<Int32>(0, new NullReferenceException(nameof(collection)));
            }

            try
            {
                ReplaceOneResult updateResult = null;
                Int64 updatedCount = 0;

                if (value is IEnumerable<IdentityEntity>)
                {
                    foreach (var entity in value as IEnumerable<IdentityEntity>)
                    {
                        updateResult = tx != null ?
                            await collection.ReplaceOneAsync(tx, x => (x as IdentityEntity).MongoId == entity.MongoId, entity, new UpdateOptions() { IsUpsert = false }, cancellationToken) :
                            await collection.ReplaceOneAsync(x => (x as IdentityEntity).MongoId == entity.MongoId, entity, new UpdateOptions() { IsUpsert = false }, cancellationToken);
                        updatedCount += updateResult.ModifiedCount;
                    }
                }
                else
                {
                    updateResult = tx != null ?
                        await collection.ReplaceOneAsync(tx, x => (x as IdentityEntity).MongoId == (value as IdentityEntity).MongoId, value, new UpdateOptions() { IsUpsert = false }, cancellationToken) :
                        await collection.ReplaceOneAsync(x => (x as IdentityEntity).MongoId == (value as IdentityEntity).MongoId, value, new UpdateOptions() { IsUpsert = false }, cancellationToken);
                    updatedCount = updateResult.ModifiedCount;
                }

                return new XResult<Int32>(Convert.ToInt32(updatedCount));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return new XResult<Int32>(0, ex);
            }
        }

        private XResult<Int32> PersistDelete(Object value, IClientSessionHandle tx = null)
        {
            var entityType = GetEntityType(value);
            if (entityType == null)
            {
                return new XResult<Int32>(0, new ArgumentNullException(nameof(entityType)));
            }

            var collection = this.GetCollection(entityType);
            if (collection == null)
            {
                return new XResult<Int32>(0, new NullReferenceException(nameof(collection)));
            }

            try
            {
                DeleteResult deleteResult = null;
                Int64 deletedCount = 0;

                if (value is IEnumerable<IdentityEntity>)
                {
                    var ids = (value as IEnumerable<IdentityEntity>).Select(x => x.MongoId);
                    deleteResult = tx != null ?
                        collection.DeleteMany(tx, x => ids.Contains((x as IdentityEntity).MongoId)) :
                        collection.DeleteMany(x => ids.Contains((x as IdentityEntity).MongoId));
                    deletedCount += deleteResult.DeletedCount;
                }
                else
                {
                    deleteResult = tx != null ?
                        collection.DeleteMany(tx, x => (x as IdentityEntity).MongoId == (value as IdentityEntity).MongoId) :
                        collection.DeleteMany(x => (x as IdentityEntity).MongoId == (value as IdentityEntity).MongoId);
                    deletedCount = deleteResult.DeletedCount;
                }

                return new XResult<Int32>(Convert.ToInt32(deletedCount));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return new XResult<Int32>(0, ex);
            }
        }

        private async Task<XResult<Int32>> PersistDeleteAsync(Object value, CancellationToken cancellationToken = default(CancellationToken), IClientSessionHandle tx = null)
        {
            var entityType = GetEntityType(value);
            if (entityType == null)
            {
                return new XResult<Int32>(0, new ArgumentNullException(nameof(entityType)));
            }

            var collection = this.GetCollection(entityType);
            if (collection == null)
            {
                return new XResult<Int32>(0, new NullReferenceException(nameof(collection)));
            }

            try
            {
                DeleteResult deleteResult = null;
                Int64 deletedCount = 0;

                if (value is IEnumerable<IdentityEntity>)
                {
                    var ids = (value as IEnumerable<IdentityEntity>).Select(x => x.MongoId);
                    deleteResult = tx != null ?
                        await collection.DeleteManyAsync(tx, x => ids.Contains((x as IdentityEntity).MongoId), null, cancellationToken) :
                        await collection.DeleteManyAsync(x => ids.Contains((x as IdentityEntity).MongoId), null, cancellationToken);
                    deletedCount += deleteResult.DeletedCount;
                }
                else
                {
                    deleteResult = tx != null ?
                        await collection.DeleteManyAsync(tx, x => (x as IdentityEntity).MongoId == (value as IdentityEntity).MongoId, null, cancellationToken) :
                        await collection.DeleteManyAsync(x => (x as IdentityEntity).MongoId == (value as IdentityEntity).MongoId, null, cancellationToken);
                    deletedCount = deleteResult.DeletedCount;
                }

                return new XResult<Int32>(Convert.ToInt32(deletedCount));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return new XResult<Int32>(0, ex);
            }
        }

        private Type GetEntityType(Object uoValue)
        {
            if (uoValue is IEnumerable)
            {
                var first = (uoValue as IEnumerable<Object>).FirstOrDefault();
                if (first != null)
                {
                    return first.GetType();
                }
            }
            else
            {
                return uoValue.GetType();
            }

            return null;
        }
    }
}
