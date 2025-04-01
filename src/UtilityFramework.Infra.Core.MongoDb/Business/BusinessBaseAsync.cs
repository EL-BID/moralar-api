using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using UtilityFramework.Infra.Core.MongoDb.Data;
using UtilityFramework.Infra.Core.MongoDb.Data.Database;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;
// ReSharper disable InconsistentNaming
// ReSharper disable once StaticMemberInGenericType
// ReSharper disable MethodOverloadWithOptionalParameter
#pragma warning disable 168
#pragma warning disable 618

namespace UtilityFramework.Infra.Core.MongoDb.Business
{
    public class BusinessBaseAsync<T> : IBusinessBaseAsync<T> where T : ModelBase
    {
        private BaseSettings _settingsDataBase;
        private readonly MongoDatabase _db;
        private readonly IMongoDatabase _dbAsync;
        public double _earthRadius = Metric.Kilometers;
        public FilterDefinition<T> _filter;
        public IMongoQuery _query;
        public double _radius = 0.1349892008639309; //15 KM
        public double _radiusMin = 0; //0 KM
        public UpdateBuilder<T> _update;
        private readonly Collation defaultCollationIgnoreCase = new Collation("en", strength: CollationStrength.Primary);

        public BusinessBaseAsync(IHostingEnvironment env)
        {
            BaseSettings.IsDev = env.IsDevelopment();

            _settingsDataBase = AppSettingsBase.GetSettings();


            var mongoClient = AppSettingsBase.GetMongoClient(_settingsDataBase);


            if (BaseSettings.MongoClient == null)
            {
                BaseSettings.MongoClient = mongoClient;
            }

            _dbAsync = mongoClient.GetDatabase(_settingsDataBase.Name);

            var server = mongoClient.GetServer();

            _db = server.GetDatabase(_settingsDataBase.Name);

            try
            {
                var propertyInfos = typeof(T).GetTypeInfo().GetProperties().Where(x => x.GetCustomAttributes(typeof(MongoIndex), false)?.FirstOrDefault() != null).ToList();

                for (int i = 0; i < propertyInfos.Count; i++)
                {
                    GetCollection().CreateIndex(propertyInfos[i].Name);
                }

            }
            catch (Exception) {/*UNUSED*/}
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }

        /// <summary>
        ///     return radius
        /// </summary>
        /// <param name="radius">VALUE IN KM</param>
        /// <returns></returns>
        public double CalculateRadius(double radius)
        {
            return Math.Abs(radius) < 0 ? 0 : radius / 111.12;
        }

        /// <summary>
        ///     get servers configuration
        /// </summary>
        /// <returns></returns>
        private IEnumerable<MongoServerAddress> ListServers()
        {
            var servers = new List<MongoServerAddress>
            {
                BaseSettings.IsDev
                    ? new MongoServerAddress(_settingsDataBase.Remote, 27017)
                    : new MongoServerAddress(_settingsDataBase.Local, 27017)
            };

            return servers;
        }


        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<string> CreateAsync(T entity)
        {
            try
            {
                await GetCollectionAsync().InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Registro não gerado", ex);
            }

            if (entity._id == ObjectId.Empty)
                throw new Exception("Registro não gerado");

            return entity._id.ToString();
        }

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task CreateAsync(IList<T> entity)
        {
            try
            {
                await GetCollectionAsync().InsertManyAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Registro não gerado", ex);
            }
        }

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> CreateReturnAsync(T entity)
        {
            try
            {
                await GetCollectionAsync().InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Registro não gerado", ex);
            }

            if (entity._id == ObjectId.Empty)
                throw new Exception("Registro não gerado");
            return entity;
        }

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IList<T>> CreateReturnAsync(IList<T> entity)
        {
            try
            {
                await GetCollectionAsync().InsertManyAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Registro não gerado", ex);
            }

            if (entity.FirstOrDefault()?._id == ObjectId.Empty)
                throw new Exception("Registro não gerado");
            return entity;
        }

        /// <summary>
        ///     UPDATE ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="setLastUpdate"></param>
        /// <returns></returns>
        public async Task<string> UpdateOneAsync(T entity, bool setLastUpdate = true)
        {
            try
            {
                if (setLastUpdate)
                    entity.LastUpdate = DateTimeOffset.Now.ToUnixTimeSeconds();

                await GetCollectionAsync().ReplaceOneAsync(x => x._id == entity._id, entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado: " + ex);
            }

            if (entity._id == ObjectId.Empty)
                throw new Exception("Não Atualizado (_id: empty) ");

            return entity._id.ToString();
        }

        /// <summary>
        ///     UPDATE ENTITY AND  return
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="setLastUpdate"></param>
        /// <returns></returns>
        public async Task<T> UpdateAsync(T entity, bool setLastUpdate = true)
        {
            try
            {
                if (setLastUpdate)
                    entity.LastUpdate = DateTimeOffset.Now.ToUnixTimeSeconds();

                await GetCollectionAsync().ReplaceOneAsync(Builders<T>.Filter.Where(x => x._id == entity._id), entity);
                return await GetCollectionAsync().FindSync(x => x._id == entity._id).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado", ex);
            }
        }

        /// <summary>
        ///     DELETE FROM ID
        /// </summary>
        /// <param name="id"></param>
        public async Task<long> DeleteOneAsync(string id)
        {
            try
            {
                var result = await GetCollectionAsync().DeleteOneAsync(x => x._id == new ObjectId(id));
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado", ex);
            }
        }

        /// <summary>
        ///     DELETE ALL FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        public async Task<long> DeleteAsync(Expression<Func<T, bool>> condition)
        {
            try
            {
                var result = await GetCollectionAsync().DeleteManyAsync(condition);

                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado", ex);
            }
        }

        /// <summary>
        ///     CHECK CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<bool> CheckByAsync(Expression<Func<T, bool>> condition)
        {
            return await GetCollectionAsync().FindSync(condition, new FindOptions<T>() { Collation = defaultCollationIgnoreCase }).AnyAsync();
        }

        /// <summary>
        ///     SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DisableOneAsync(string id)
        {
            try
            {
                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                var builder = Builders<T>.Update;
                var update = builder.Set(x => x.Disabled, now).Set(x => x.LastUpdate, now);

                await GetCollectionAsync().UpdateOneAsync(x => x._id == new ObjectId(id), update);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> EnableOneAsync(string id)
        {
            try
            {
                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                var builder = Builders<T>.Update;
                var update = builder.Set(x => x.Disabled, null).Set(x => x.LastUpdate, now);

                await GetCollectionAsync().UpdateOneAsync(x => x._id == new ObjectId(id), update);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByContais(string paramName, string value)
        {
            _filter = string.IsNullOrEmpty(value) ? Builders<T>.Filter.Empty : Builders<T>.Filter.Regex(paramName, new BsonRegularExpression(value, "i"));

            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T>() { Collation = defaultCollationIgnoreCase }).ToListAsync();
        }

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByContais(Expression<Func<T, bool>> condition, string paramName,
            string value)
        {
            _filter = string.IsNullOrEmpty(value) ?
                Builders<T>.Filter.And(Builders<T>.Filter.Where(condition)) :
                Builders<T>.Filter.And(Builders<T>.Filter.Where(condition), Builders<T>.Filter.Regex(paramName, new BsonRegularExpression(value, "i")));

            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T>() { Collation = defaultCollationIgnoreCase }).ToListAsync();
        }

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByContais(Expression<Func<T, bool>> condition, string paramName,
            string value, SortDefinition<T> sortBy)
        {
            _filter =
                string.IsNullOrEmpty(value) ?
                Builders<T>.Filter.And(Builders<T>.Filter.Where(condition)) :
                Builders<T>.Filter.And(Builders<T>.Filter.Where(condition), Builders<T>.Filter.Regex(paramName, new BsonRegularExpression(value, "i")));




            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy }).ToListAsync();
        }

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByContais(Expression<Func<T, bool>> condition, string paramName,
            string value, SortDefinition<T> sortBy, int limit = 30)
        {
            _filter =
                string.IsNullOrEmpty(value) ?
                Builders<T>.Filter.And(Builders<T>.Filter.Where(condition)) :
                Builders<T>.Filter.And(Builders<T>.Filter.Where(condition),
                    Builders<T>.Filter.Regex(paramName, new BsonRegularExpression(value, "i")));




            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy }).ToListAsync();
        }

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByContais(Expression<Func<T, bool>> condition, string paramName,
            string value, SortDefinition<T> sortBy, int page, int limit = 30)
        {
            _filter = string.IsNullOrEmpty(value) ?
                Builders<T>.Filter.And(Builders<T>.Filter.Where(condition)) : Builders<T>.Filter.And(Builders<T>.Filter.Where(condition),
                    Builders<T>.Filter.Regex(paramName, new BsonRegularExpression(value, "i")));

            var skip = ((page < 1 ? 1 : page) - 1) * limit;




            return await GetCollectionAsync()
                .FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy, Limit = limit, Skip = skip }).ToListAsync();
        }

        /// <summary>
        ///     FIND ALL IN
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindIn<TY>(string paramName, List<TY> value)
        {
            _filter = value.Count == 0 ? Builders<T>.Filter.Empty : Builders<T>.Filter.In(paramName, value);

            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T>() { Collation = defaultCollationIgnoreCase }).ToListAsync();
        }

        /// <summary>
        ///     FIND IN
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindIn<TY>(Expression<Func<T, bool>> condition, string paramName,
            List<TY> value, SortDefinition<T> sortBy)
        {
            _filter = value.Count() == 0 ? Builders<T>.Filter.Where(condition) : Builders<T>.Filter.And(Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.In(paramName, value));




            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy }).ToListAsync();
        }

        /// <summary>
        ///     FIND IN
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindIn<TY>(Expression<Func<T, bool>> condition, string paramName,
            List<TY> value, SortDefinition<T> sortBy, int limit = 30)
        {
            _filter = value.Count() == 0 ? Builders<T>.Filter.Where(condition) : Builders<T>.Filter.And(Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.In(paramName, value));





            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy, Limit = limit })
                .ToListAsync();
        }

        /// <summary>
        ///     FIND IN
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindIn<TY>(Expression<Func<T, bool>> condition, string paramName,
            List<TY> value, SortDefinition<T> sortBy, int page, int limit = 30)
        {
            _filter = value.Count == 0 ? Builders<T>.Filter.Where(condition) : Builders<T>.Filter.And(Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.In(paramName, value));

            var skip = ((page < 1 ? 1 : page) - 1) * limit;




            return await GetCollectionAsync()
                .FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy, Skip = skip, Limit = limit }).ToListAsync();
        }

        /// <summary>
        ///     FIND ENDITY FROM ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> FindByIdAsync(string id/**, bool ignoreDisabled=true  **/)
        {
            return await GetCollectionAsync().FindSync(x => x._id == new ObjectId(id)).SingleOrDefaultAsync();
        }

        /// <summary>
        ///     FIND ONE ENTITY FROM CONDITION
        /// </summary>
        /// <param name="pCondition"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<T> FindOneByAsync(Expression<Func<T, bool>> pCondition /**, bool ignoreDisabled=true  **/)
        {
            //var condition = ignoreDisabled ? pCondition :
            return await GetCollectionAsync().FindSync(pCondition, new FindOptions<T>() { Collation = defaultCollationIgnoreCase }).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     FIND ALL ENTITY
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAllAsync()
        {
            var filter = Builders<T>.Filter.Empty;

            return await GetCollectionAsync().FindSync(filter, new FindOptions<T>() { Collation = defaultCollationIgnoreCase }).ToListAsync();
        }

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAllAsync(SortDefinition<T> sortBy)
        {
            var filter = Builders<T>.Filter.Empty;




            return await GetCollectionAsync().FindSync(filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy }).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(SortDefinition<T> sortBy, int page, int limit = 30)
        {
            var filter = Builders<T>.Filter.Empty;

            var skip = ((page < 1 ? 1 : page) - 1) * limit;




            return await GetCollectionAsync().FindSync(filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy, Limit = limit, Skip = skip }).ToListAsync();
        }

        /// <summary>
        ///     FIND BY CONDITION SIMPLE
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> condition)
        {
            return await GetCollectionAsync().FindSync(condition, new FindOptions<T>() { Collation = defaultCollationIgnoreCase }).ToListAsync();
        }

        /// <summary>
        ///     FIND BY CONDITION WITH ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> condition, SortDefinition<T> sortBy)
        {



            return await GetCollectionAsync().FindSync(condition, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy }).ToListAsync();
        }

        /// <summary>
        ///     FINDY BY CONDITION WITH PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> condition, int page, int limit = 30)
        {
            var skip = ((page < 1 ? 1 : page) - 1) * limit;
            return await GetCollectionAsync().FindSync(condition, new FindOptions<T> { Limit = limit, Skip = skip })
                .ToListAsync();
        }

        /// <summary>
        ///     FIND BY CONDITION WITH SORTBY AND PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> condition, int page,
            SortDefinition<T> sortBy, int limit = 30)
        {


            var skip = ((page < 1 ? 1 : page) - 1) * limit;
            return await GetCollectionAsync()
                .FindSync(condition, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy, Limit = limit, Skip = skip }).ToListAsync();
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByNearAsync(double lat, double lng, double maxDistance,
            string propertyName = "Location", IEnumerable<FilterDefinition<T>> queries = null, double minDistance = 0, bool isLngLat = false)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);
            _radiusMin = CalculateRadius(minDistance);

            if (GetCollection().IndexExists(IndexKeys.GeoSpatial(propertyName)) == false)
                GetCollection().CreateIndex(IndexKeys.GeoSpatial(propertyName));

            var filterCondition = isLngLat == false
                            ? Builders<T>.Filter.Near(propertyName, lat, lng, _radius, _radiusMin)
                            : Builders<T>.Filter.Near(propertyName, lng, lat, _radius, _radiusMin);

            var listQueries = new List<FilterDefinition<T>>() { filterCondition };

            if (queries != null)
                listQueries.AddRange(queries);




            _filter = Builders<T>.Filter.And(listQueries);
            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T>() { Collation = defaultCollationIgnoreCase }).ToListAsync();
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByNearAsync(double lat, double lng, double maxDistance, int page,
            int limit = 30, string propertyName = "Location",
            IEnumerable<FilterDefinition<T>> queries = null, double minDistance = 0, bool isLngLat = false)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);
            _radiusMin = CalculateRadius(minDistance);

            var skip = ((page < 1 ? 1 : page) - 1) * limit;

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatial(propertyName)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatial(propertyName));

            var filterCondition = isLngLat
            ? Builders<T>.Filter.Near(propertyName, lat, lng, _radius, _radiusMin)
            : Builders<T>.Filter.Near(propertyName, lng, lat, _radius, _radiusMin);

            var listQueries = new List<FilterDefinition<T>> {
                Builders<T>.Filter.Near (propertyName, lat, lng, _radius, _radiusMin)
            };

            if (queries != null)
                listQueries.AddRange(queries);




            _filter = Builders<T>.Filter.And(listQueries);
            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Limit = limit, Skip = skip })
                .ToListAsync();
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITIONS AND SORTBY
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="sortBy"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByNearAsync(double lat, double lng, double maxDistance,
            SortDefinition<T> sortBy, string propertyName = "Location", IEnumerable<FilterDefinition<T>> queries = null, double minDistance = 0, bool isLngLat = false)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);
            _radiusMin = CalculateRadius(minDistance);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatial(propertyName)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatial(propertyName));

            var filterCondition = isLngLat == false
            ? Builders<T>.Filter.Near(propertyName, lat, lng, _radius, _radiusMin)
            : Builders<T>.Filter.Near(propertyName, lng, lat, _radius, _radiusMin);

            var listQueries = new List<FilterDefinition<T>> { filterCondition };

            if (queries != null)
                listQueries.AddRange(queries);




            _filter = Builders<T>.Filter.And(listQueries);
            return await GetCollectionAsync().FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy }).ToListAsync();
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITIONS AND SORTBY
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="sortBy"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByNearAsync(double lat, double lng, double maxDistance,
            SortDefinition<T> sortBy, int page, int limit = 30, string propertyName = "Location",
            IEnumerable<FilterDefinition<T>> queries = null, double minDistance = 0, bool isLngLat = false)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);
            _radiusMin = CalculateRadius(minDistance);

            var skip = ((page < 1 ? 1 : page) - 1) * limit;
            if (!GetCollection().IndexExists(IndexKeys.GeoSpatial(propertyName)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatial(propertyName));

            var filterCondition = isLngLat == false
            ? Builders<T>.Filter.Near(propertyName, lat, lng, _radius, _radiusMin)
            : Builders<T>.Filter.Near(propertyName, lng, lat, _radius, _radiusMin);

            var listQueries = new List<FilterDefinition<T>> { filterCondition };

            if (queries != null)
                listQueries.AddRange(queries);




            _filter = Builders<T>.Filter.And(listQueries);
            return await GetCollectionAsync()
                .FindSync(_filter, new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy, Skip = skip, Limit = limit }).ToListAsync();
        }

        /// <summary>
        ///     COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> condition)
        {
            return (int)await GetCollectionAsync().CountAsync(condition);
        }

        /// <summary>
        ///     COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<long> CountLongAsync(Expression<Func<T, bool>> condition)
        {
            return await GetCollectionAsync().CountAsync(condition);
        }

        public async Task<IEnumerable<T>> LoadDataTableAsync(string searchTerm, SortDefinition<T> sortBy, int skip,
            int limit = 10, params string[] fields)
        {

            var listQueries = SearchCondition(searchTerm, fields);

            var builder = Builders<T>.Filter;

            var customFilter = listQueries.Count > 0
            ? builder.And(builder.Or(listQueries))
            : builder.Empty;

            var options = new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy };

            if (skip > 0)
                options.Skip = skip;
            if (limit > 0)
                options.Limit = limit;

            return await GetCollectionAsync()
                .FindSync(customFilter, options).ToListAsync();
        }

        public async Task<IEnumerable<T>> LoadDataTableAsync(string searchTerm, SortDefinition<T> sortBy, int skip,
            int limit = 10, IEnumerable<FilterDefinition<T>> queries = null, params string[] fields)
        {
            var listQueries = SearchCondition(searchTerm, fields);

            var customFilter = Builders<T>.Filter.Empty;
            var builder = Builders<T>.Filter;

            if (queries != null && queries.Count() > 0)
            {
                customFilter = listQueries.Count > 0
                ? Builders<T>.Filter.And(builder.And(queries), builder.And(builder.Or(listQueries)))
                : Builders<T>.Filter.And(queries);
            }
            else if (listQueries.Count > 0)
            {
                customFilter = Builders<T>.Filter.Or(listQueries);
            }
            var options = new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy };

            if (skip > 0)
                options.Skip = skip;
            if (limit > 0)
                options.Limit = limit;

            return await GetCollectionAsync()
                .FindSync(customFilter, options).ToListAsync();
        }

        public async Task<IEnumerable<T>> LoadDataTableAsync(Expression<Func<T, bool>> condition, string searchTerm,
            SortDefinition<T> sortBy, int skip, int limit = 10, params string[] fields)
        {
            var listQueries = SearchCondition(searchTerm, fields);

            var builder = Builders<T>.Filter;

            var customFilter = listQueries.Count > 0
                ? Builders<T>.Filter.And(builder.Where(condition), builder.And(builder.Or(listQueries)))
                : Builders<T>.Filter.Where(condition);

            var options = new FindOptions<T> { Collation = defaultCollationIgnoreCase, Sort = sortBy };

            if (skip > 0)
                options.Skip = skip;
            if (limit > 0)
                options.Limit = limit;

            return await GetCollectionAsync()
                .FindSync(customFilter, options).ToListAsync();
        }

        public async Task<long> CountSearchDataTableAsync(string searchTerm, params string[] fields)
        {
            var listQueries = SearchCondition(searchTerm, fields);

            var custonConditions = listQueries.Count > 0
            ? Builders<T>.Filter.Or(listQueries)
            : Builders<T>.Filter.Empty;

            return await GetCollectionAsync().CountAsync(custonConditions);
        }

        public async Task<long> CountSearchDataTableAsync(string searchTerm,
            IEnumerable<FilterDefinition<T>> queries = null, params string[] fields)
        {
            var listQueries = SearchCondition(searchTerm, fields);

            var customFilter = Builders<T>.Filter.Empty;
            var builder = Builders<T>.Filter;

            if (queries != null && queries.Count() > 0)
            {
                customFilter = listQueries.Count > 0
                ? builder.And(builder.And(queries), builder.And(builder.Or(listQueries)))
                : builder.And(queries);
            }
            else if (listQueries.Count > 0)
            {
                customFilter = builder.And(builder.Or(listQueries));
            }

            return await GetCollectionAsync().CountAsync(customFilter);
        }

        public async Task<long> CountSearchDataTableAsync(Expression<Func<T, bool>> condition, string searchTerm,
            params string[] fields)
        {
            var listQueries = SearchCondition(searchTerm, fields);

            var builder = Builders<T>.Filter;

            var custonConditions = listQueries.Count > 0

            ? builder.And(builder.Where(condition), builder.And(builder.Or(listQueries)))
            : builder.Where(condition);

            return await GetCollectionAsync().CountAsync(custonConditions);
        }

        #region Connection

        /// <summary>
        ///     collection off documents
        /// </summary>
        /// <returns></returns>
        public IMongoCollection<T> GetCollectionAsync()
        {
            // Create instance
            var entity = Activator.CreateInstance<T>();

            //  return instance
            return _dbAsync.GetCollection<T>(entity.CollectionName);
        }

        /// <summary>
        ///     collection off documents
        /// </summary>
        /// <returns></returns>
        public MongoCollection<T> GetCollection()
        {
            // Create instance
            var entity = Activator.CreateInstance<T>();

            //  return instance
            return _db.GetCollection<T>(entity.CollectionName);
        }

        #endregion

        #region Sync Methods

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string Create(T entity)
        {
            try
            {
                GetCollection().Save(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Registro não gerado");
            }

            if (entity._id == ObjectId.Empty)
                throw new Exception("Registro não gerado");
            return entity._id.ToString();
        }

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Create(IList<T> entity)
        {
            try
            {
                GetCollection().InsertBatch(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Registro não gerado");
            }
        }

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T CreateReturn(T entity)
        {
            try
            {
                GetCollection().Save(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Registro não gerado");
            }

            if (entity._id == ObjectId.Empty)
                throw new Exception("Registro não gerado");

            return entity;
        }

        /// <summary>
        ///     UPDATE ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string UpdateOne(T entity)
        {
            try
            {
                entity.LastUpdate = DateTimeOffset.Now.ToUnixTimeSeconds();

                GetCollection().Save(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado");
            }

            if (entity._id == ObjectId.Empty)
                throw new Exception("Não Atualizado");

            return entity._id.ToString();
        }

        /// <summary>
        ///     UPDATE MULTIPLE
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="update"></param>
        /// <param name="flags"></param>
        public void UpdateMultiple(IMongoQuery condition, UpdateBuilder<T> update, UpdateFlags flags)
        {
            try
            {
                GetCollection().Update(condition, update, flags);
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado");
            }
        }

        /// <summary>
        ///     UPDATE ENTITY AND  return
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Update(T entity)
        {
            try
            {
                entity.LastUpdate = DateTimeOffset.Now.ToUnixTimeSeconds();

                GetCollection().Save(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado");
            }

            if (entity._id == ObjectId.Empty)
                throw new Exception("Não Atualizado");

            return GetCollection().FindOneById(entity._id);
        }

        /// <summary>
        ///     DELETE FROM ID
        /// </summary>
        /// <param name="id"></param>
        public void DeleteOne(string id)
        {
            _query = Query<T>.EQ(x => x._id, new ObjectId(id));

            GetCollection().Remove(_query);
        }

        /// <summary>
        ///     DELETE ALL FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        public void Delete(Expression<Func<T, bool>> condition)
        {
            _query = Query<T>.Where(condition);

            GetCollection().Remove(_query);
        }

        public void Delete(T entity)
        {
            _query = Query<T>.EQ(x => x._id, entity._id);

            GetCollection().Remove(_query);
        }

        /// <summary>
        ///     CHECK CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool CheckBy(Expression<Func<T, bool>> condition)
        {
            _query = Query<T>.Where(condition);

            return GetCollection().Find(_query).Count() > 0;
        }

        /// <summary>
        ///     SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DisableOne(string id)
        {
            try
            {
                _query = Query<T>.EQ(x => x._id, new ObjectId(id));
                _update = new UpdateBuilder<T>();
                _update.Set(x => x.Disabled, DateTimeOffset.Now.ToUnixTimeSeconds());
                _update.Set(x => x.LastUpdate, DateTimeOffset.Now.ToUnixTimeSeconds());

                GetCollection().Update(_query, _update);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EnableOne(string id)
        {
            try
            {
                _query = Query<T>.EQ(x => x._id, new ObjectId(id));
                _update = new UpdateBuilder<T>();
                _update.Set(x => x.Disabled, null);
                _update.Set(x => x.LastUpdate, DateTimeOffset.Now.ToUnixTimeSeconds());

                GetCollection().Update(_query, _update);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     FIND ENDITY FROM ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById(string id)
        {
            return GetCollection().FindOneById(new ObjectId(id));
        }

        /// <summary>
        ///     FIND ONE ENTITY FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T FindOneBy(Expression<Func<T, bool>> condition)
        {
            _query = Query<T>.Where(condition);
            return GetCollection().FindOne(_query);
        }

        /// <summary>
        ///     FIND ALL ENTITY
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FindAll()
        {
            return GetCollection().FindAll().SetCollation(defaultCollationIgnoreCase);
        }

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public IEnumerable<T> FindAll(IMongoSortBy sortBy)
        {
            return GetCollection().FindAll().SetCollation(defaultCollationIgnoreCase).SetSortOrder(sortBy);
        }

        /// <summary>
        ///     FIND BY CONDITION SIMPLE
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> condition)
        {
            _query = Query<T>.Where(condition);
            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase);
        }

        /// <summary>
        ///     FIND BY CONDITION WITH ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, IMongoSortBy sortBy)
        {
            _query = Query<T>.Where(condition);
            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSortOrder(sortBy);
        }

        /// <summary>
        ///     FINDY BY CONDITION WITH PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, int page, int limit = 30)
        {
            _query = Query<T>.Where(condition);
            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSkip(((page < 1 ? 1 : page) - 1) * limit).SetLimit(limit);
        }

        /// <summary>
        ///     FIND BY CONDITION WITH SORTBY AND PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, int page, IMongoSortBy sortBy, int limit = 30)
        {
            _query = Query<T>.Where(condition);
            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSortOrder(sortBy).SetSkip(((page < 1 ? 1 : page) - 1) * limit)
                .SetLimit(limit);
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, string propertyName = "Location",
            IEnumerable<IMongoQuery> queries = null, bool isLngLat = false)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatial(propertyName)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatial(propertyName));

            var filterCondition = isLngLat == false ? Query.Near(propertyName, lat, lng, _radius) : Query.Near(propertyName, lng, lat, _radius);

            var listQueries = new List<IMongoQuery>() { filterCondition };

            if (queries != null)
                listQueries.AddRange(queries);

            _query = Query.And(listQueries);

            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).ToList();
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, int page, int limit = 30,
            string propertyName = "Location",
            IEnumerable<IMongoQuery> queries = null, bool isLngLat = false)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatial(propertyName)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatial(propertyName));

            var listQueries = new List<IMongoQuery> {
                isLngLat == false ? Query.Near(propertyName, lat, lng, _radius) : Query.Near(propertyName, lng, lat, _radius)
            };

            if (queries != null)
                listQueries.AddRange(queries);

            _query = Query.And(listQueries);

            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSkip(((page < 1 ? 1 : page) - 1) * limit).SetLimit(limit);
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITIONS AND SORTBY
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="sortBy"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, IMongoSortBy sortBy,
            string propertyName = "Location", IEnumerable<IMongoQuery> queries = null, bool isLngLat = false)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatial(propertyName)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatial(propertyName));
            var filterCondition = isLngLat == false ? Query.Near(propertyName, lat, lng, _radius) : Query.Near(propertyName, lng, lat, _radius);

            var listQueries = new List<IMongoQuery>() { filterCondition };

            if (queries != null)
                listQueries.AddRange(queries);

            _query = Query.And(listQueries);




            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSortOrder(sortBy).ToList();
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITIONS AND SORTBY
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, IMongoSortBy sortBy, int page,
            int limit = 30,
            string propertyName = "Location", IEnumerable<IMongoQuery> queries = null, bool isLngLat = false)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatial(propertyName)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatial(propertyName));

            var filterCondition = isLngLat == false
            ? Query.Near(propertyName, lat, lng, _radius)
            : Query.Near(propertyName, lng, lat, _radius);
            var listQueries = new List<IMongoQuery> {
                Query.Near (propertyName, lat, lng, _radius)
            };

            if (queries != null)
                listQueries.AddRange(queries);

            _query = Query.And(listQueries);


            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSortOrder(sortBy).SetSkip(((page < 1 ? 1 : page) - 1) * limit)
                .SetLimit(limit);
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION AND  return AND  return DISTANCE
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="typeMetric"></param>
        /// <param name="spherical"></param>
        /// <param name="propertyIndex"></param>
        /// <param name="distanceProperty"></param>
        /// <param name="queries"></param>
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public IEnumerable<T> FindByNearWithDistance(double lat, double lng, double maxDistance,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, double? minDistance = null, bool isLngLat = false)
        {
            var listQueries = new List<IMongoQuery>();

            if (queries != null)
                listQueries.AddRange(queries.ToList());

            double.TryParse(typeof(Metric).GetProperty(Enum.GetName(typeof(TypeMetric), (int)typeMetric)).GetValue(this, null).ToString(), out _earthRadius);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatialSpherical(propertyIndex)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatialSpherical(propertyIndex));

            var stage = GenerateGeonearStage<T>(lat, lng, listQueries, distanceProperty, maxDistance, _earthRadius, typeMetric, minDistance, isLngLat);




            return GetCollectionAsync().Aggregate(new AggregateOptions() { Collation = defaultCollationIgnoreCase }).AppendStage(stage).ToEnumerable();


        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION AND  return AND  return DISTANCE
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="typeMetric"></param>
        /// <param name="spherical"></param>
        /// <param name="propertyIndex"></param>
        /// <param name="distanceProperty"></param>
        /// <param name="queries"></param>
        /// <param name="limit"></param>
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        public IEnumerable<T> FindByNearWithDistance(double lat, double lng, double maxDistance, int page,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, int limit = 30, double? minDistance = null, bool isLngLat = false)
        {
            var listQueries = new List<IMongoQuery>();

            if (queries != null)
                listQueries.AddRange(queries.ToList());

            double.TryParse(
                typeof(Metric).GetProperty(Enum.GetName(typeof(TypeMetric), (int)typeMetric)).GetValue(this, null)
                .ToString(), out _earthRadius);

            if (GetCollection().IndexExists(IndexKeys.GeoSpatialSpherical(propertyIndex)) == false)
                GetCollection().CreateIndex(IndexKeys.GeoSpatialSpherical(propertyIndex));

            var stage = GenerateGeonearStage<T>(lat, lng, listQueries, distanceProperty, maxDistance, _earthRadius, typeMetric, minDistance, isLngLat);




            return GetCollectionAsync().Aggregate().AppendStage(stage).Skip((page - 1) * limit).Limit(limit).ToList();
        }

        /// <summary>
        ///     COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> condition)
        {
            return (int)GetCollectionAsync().Count(condition);
        }

        /// <summary>
        ///     COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public long CountLong(Expression<Func<T, bool>> condition)
        {
            _query = Query<T>.Where(condition);
            return GetCollection().Count(_query);
        }

        public IEnumerable<T> LoadDataTable(string searchTerm, IMongoSortBy sortby, int skip, int limit = 10,
            params string[] fields)
        {
            var conditions = new List<IMongoQuery>();

            if (string.IsNullOrEmpty(searchTerm) == false)
                conditions.AddRange(fields.Select(t => Query.Matches(FirstCharToUpper(t), new BsonRegularExpression(searchTerm, "i"))));

            _query = conditions.Count > 0 ? Query.Or(conditions) : Query<T>.Where(x => x.Created != null);

            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSortOrder(sortby).SetSkip(skip).SetLimit(limit);
        }

        public IEnumerable<T> LoadDataTable(string searchTerm, IMongoSortBy sortby, int skip, int limit = 10,
            IEnumerable<IMongoQuery> queries = null, params string[] fields)
        {
            var conditions = new List<IMongoQuery>();

            if (string.IsNullOrEmpty(searchTerm) == false)
                conditions.AddRange(fields.Select(t => Query.Matches(t, new BsonRegularExpression(searchTerm, "i"))));

            _query = conditions.Count > 0 ? Query.And(Query.And(queries), Query.Or(conditions)) : Query.And(queries);

            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSortOrder(sortby).SetSkip(skip).SetLimit(limit);
        }

        public IEnumerable<T> LoadDataTable(Expression<Func<T, bool>> condition, string searchTerm, IMongoSortBy sortby,
            int skip, int limit = 10, params string[] fields)
        {
            var conditions = new List<IMongoQuery>();
            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                conditions.AddRange(fields.Select(t => Query.Matches(t, new BsonRegularExpression(searchTerm, "i"))));
            }

            _query = conditions.Count > 0 ? Query.And(Query<T>.Where(condition), Query.Or(conditions)) : Query<T>.Where(condition);

            return GetCollection().Find(_query).SetCollation(defaultCollationIgnoreCase).SetSortOrder(sortby).SetSkip(skip).SetLimit(limit);
        }

        public long CountSearchDataTable(string searchTerm, params string[] fields)
        {
            var conditions = new List<IMongoQuery>();

            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                conditions.AddRange(fields.Select(field =>
                  Query.Matches(field, new BsonRegularExpression(searchTerm, "i"))));
            }

            _query = conditions.Count > 0 ? Query.Or(conditions) : Query.Empty;

            return GetCollection().Count(_query);
        }

        public long CountSearchDataTable(string searchTerm, IEnumerable<IMongoQuery> queries = null,
            params string[] fields)
        {
            var conditions = new List<IMongoQuery>();

            if (string.IsNullOrEmpty(searchTerm) == false)
                conditions.AddRange(fields.Select(field => Query.Matches(FirstCharToUpper(field), new BsonRegularExpression(searchTerm, "i"))));
            _query = conditions.Count > 0 ? Query.And(Query.And(queries), Query.And(Query.Or(conditions))) : Query.And(queries);

            return GetCollection().Count(_query);
        }

        public long CountSearchDataTable(Expression<Func<T, bool>> condition, string searchTerm, params string[] fields)
        {
            var conditions = new List<IMongoQuery>();

            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                conditions.AddRange(fields.Select(field =>
                  Query.Matches(field, new BsonRegularExpression(searchTerm, "i"))));
            }
            _query = conditions.Count > 0 ? Query.And(Query<T>.Where(condition), Query.And(Query.Or(conditions))) : Query<T>.Where(condition);

            return GetCollection().Count(_query);
        }

        #endregion

        private List<FilterDefinition<T>> SearchCondition(string searchTerm, params string[] fields)
        {
            var listConditions = new List<FilterDefinition<T>>();
            if (string.IsNullOrEmpty(searchTerm) || fields == null || fields.Length < 1)
                return listConditions;

            var properties = GetPropertiesName(typeof(T));

            listConditions.AddRange(properties.Where(x => fields.Count(y => string.IsNullOrEmpty(y) == false && string.Equals(x, y, StringComparison.OrdinalIgnoreCase)) > 0)
            .Select(t => ReturnFilter(t, searchTerm)));

            return listConditions;
        }

        private FilterDefinition<T> ReturnFilter(string field, string searchTerm)
        {

            if ((field.ToString() == "_id" || field.ToLower() == "id") && ObjectId.TryParse(searchTerm, out ObjectId _id))
                return Builders<T>.Filter.Eq(x => x._id, _id);
            else
                return Builders<T>.Filter.Regex(field, new BsonRegularExpression(searchTerm, "i"));
        }

        public string PrintQuery(FilterDefinition<T> filter)
        {

            return filter.Render(GetCollectionAsync().DocumentSerializer,
                GetCollectionAsync().Settings.SerializerRegistry).ToString();
        }
        private void DisposeVariables()
        {
            _filter = null;
            _filter = null;
            _query = null;
            _update = null;
        }

        private List<string> GetPropertiesName(Type data, string prefix = "")
        {
            var response = new List<string>();
            try
            {
                var properties = data.GetProperties();

                for (int i = 0; i < properties.Count(); i++)
                {
                    try
                    {
                        var propInfo = properties[i];
                        var propType = propInfo.GetType();
                        var propTypeInfo = propType.GetTypeInfo();
                        var isNullable = (Nullable.GetUnderlyingType(propInfo.PropertyType) != null);
                        var isEnum = propTypeInfo.IsEnum || propInfo.PropertyType.GetTypeInfo().IsEnum;
                        var isArray = typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType);
                        var isObjectId = propInfo.PropertyType == typeof(ObjectId);

                        var propName = prefix + (propInfo.GetCustomAttribute<BsonElementAttribute>()?.ElementName ?? propInfo.Name);
                        if (propType.GetTypeInfo().IsClass && (isObjectId == false && isNullable == false && isEnum == false && propInfo.PropertyType != typeof(string) && propInfo.PropertyType != typeof(double) && propInfo.PropertyType != typeof(decimal) && propInfo.PropertyType != typeof(long) && propInfo.PropertyType != typeof(decimal) && propInfo.PropertyType != typeof(DateTime) && propInfo.PropertyType != typeof(TimeSpan) && propInfo.PropertyType != typeof(decimal) && propInfo.PropertyType != typeof(TimeSpan) && propInfo.PropertyType != typeof(decimal) && propInfo.PropertyType != typeof(int)))
                        {
                            if (isArray)
                            {
                                response.AddRange(GetPropertiesName(propInfo.PropertyType.GetMethod("get_Item").ReturnType, $"{propName}."));
                            }
                            else
                            {
                                response.AddRange(GetPropertiesName(propInfo.PropertyType, $"{propName}."));
                            }
                        }
                        else
                        {
                            response.Add(propName);
                        }
                    }
                    catch (Exception)
                    {
                        /* ignored */
                    }


                }

            }
            catch (Exception) {/*unused*/}


            return response;

        }



        private string FirstCharToUpper(string text) => string.IsNullOrEmpty(text) == false ? char.ToUpper(text[0]) + text.Substring(1) : text;
        private BsonDocumentPipelineStageDefinition<T, T> GenerateGeonearStage<T>(double lat, double lng, List<IMongoQuery> listQueries, string distanceProperty, double maxDistance, double earthRadius, TypeMetric typeMetric, double? minDistance = null, bool isLngLat = false) where T : ModelBase
        {
            try
            {
                var location = isLngLat ? new double[] { lng, lat } : new double[] { lat, lng };

                var geoPoint = new BsonDocument
                {
                    {"type","Point"},
                    {"coordinates",new BsonArray(location)}
                };

                var geoNearOptions = new BsonDocument
                {
                    {"near", geoPoint},
                    {"distanceField", distanceProperty},
                    {"maxDistance",maxDistance * 1000},
                    {"query", listQueries.Count() > 0? Query.And(listQueries).ToBsonDocument() : new BsonDocument() },
                    {"spherical", true},
                };

                if (minDistance.GetValueOrDefault() > 0)
                    geoNearOptions.Add("maxDistance", minDistance * 1000);

                if (typeMetric == TypeMetric.Kilometers)
                    geoNearOptions.Add(new BsonElement("distanceMultiplier", 0.001));

                return new BsonDocumentPipelineStageDefinition<T, T>(new BsonDocument { { "$geoNear", geoNearOptions } });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<T>> FindByNearWithDistanceAsync(double lat, double lng, double maxDistance, TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location", string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, double? minDistance = null, bool isLngLat = false)
        {
            var listQueries = new List<IMongoQuery>();

            if (queries != null)
                listQueries.AddRange(queries.ToList());

            double.TryParse(typeof(Metric).GetProperty(Enum.GetName(typeof(TypeMetric), (int)typeMetric)).GetValue(this, null).ToString(), out _earthRadius);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatialSpherical(propertyIndex)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatialSpherical(propertyIndex));

            var stage = GenerateGeonearStage<T>(lat, lng, listQueries, distanceProperty, maxDistance, _earthRadius, typeMetric, minDistance, isLngLat);

            return await GetCollectionAsync().Aggregate(new AggregateOptions() { Collation = defaultCollationIgnoreCase }).AppendStage(stage).ToListAsync();
        }

        public async Task<List<T>> FindByNearWithDistanceAsync(double lat, double lng, double maxDistance, SortDefinition<T> sortBy, TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location", string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, double? minDistance = null, bool isLngLat = false)
        {
            var listQueries = new List<IMongoQuery>();

            if (queries != null)
                listQueries.AddRange(queries.ToList());

            double.TryParse(typeof(Metric).GetProperty(Enum.GetName(typeof(TypeMetric), (int)typeMetric)).GetValue(this, null).ToString(), out _earthRadius);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatialSpherical(propertyIndex)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatialSpherical(propertyIndex));

            var stage = GenerateGeonearStage<T>(lat, lng, listQueries, distanceProperty, maxDistance, _earthRadius, typeMetric);

            return await GetCollectionAsync().Aggregate(new AggregateOptions() { Collation = defaultCollationIgnoreCase }).AppendStage(stage).Sort(sortBy).ToListAsync();
        }

        public async Task<List<T>> FindByNearWithDistanceAsync(double lat, double lng, double maxDistance, int page, TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location", string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, int limit = 30, double? minDistance = null, bool isLngLat = false)
        {
            var listQueries = new List<IMongoQuery>();

            if (queries != null)
                listQueries.AddRange(queries.ToList());

            double.TryParse(typeof(Metric).GetProperty(Enum.GetName(typeof(TypeMetric), (int)typeMetric)).GetValue(this, null).ToString(), out _earthRadius);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatialSpherical(propertyIndex)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatialSpherical(propertyIndex));

            var stage = GenerateGeonearStage<T>(lat, lng, listQueries, distanceProperty, maxDistance, _earthRadius, typeMetric, minDistance, isLngLat);


            return await GetCollectionAsync().Aggregate(new AggregateOptions() { Collation = defaultCollationIgnoreCase }).AppendStage(stage).Skip((page - 1) * limit).Limit(limit).ToListAsync();
        }

        public async Task<List<T>> FindByNearWithDistanceAsync(double lat, double lng, double maxDistance, int page, SortDefinition<T> sortBy, TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location", string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, int limit = 30, double? minDistance = null, bool isLngLat = false)
        {
            var listQueries = new List<IMongoQuery>();

            if (queries != null)
                listQueries.AddRange(queries.ToList());

            double.TryParse(typeof(Metric).GetProperty(Enum.GetName(typeof(TypeMetric), (int)typeMetric)).GetValue(this, null).ToString(), out _earthRadius);

            if (!GetCollection().IndexExists(IndexKeys.GeoSpatialSpherical(propertyIndex)))
                GetCollection().CreateIndex(IndexKeys.GeoSpatialSpherical(propertyIndex));

            var stage = GenerateGeonearStage<T>(lat, lng, listQueries, distanceProperty, maxDistance, _earthRadius, typeMetric, minDistance, isLngLat);

            return await GetCollectionAsync().Aggregate(new AggregateOptions() { Collation = defaultCollationIgnoreCase }).AppendStage(stage).Sort(sortBy).Skip((page - 1) * limit).Limit(limit).ToListAsync();
        }
    }
}