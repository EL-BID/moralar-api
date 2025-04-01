using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using UtilityFramework.Infra.Core.MongoDb.Data;
using UtilityFramework.Infra.Core.MongoDb.Data.Database;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace UtilityFramework.Infra.Core.MongoDb.Business
{
    public class BusinessBase<T> : IBusinessBase<T> where T : ModelBase
    {
        // ReSharper disable once StaticMemberInGenericType
        private static BaseSettings _settingsDataBase;

        private readonly MongoDatabase _db;

        // ReSharper disable once InconsistentNaming
        public double _earthRadius = Metric.Kilometers;

        // ReSharper disable once InconsistentNaming
        public IMongoQuery _query;

        // ReSharper disable once InconsistentNaming
        public double _radius = 0.1349892008639309; //15 KM

        // ReSharper disable once InconsistentNaming
        public UpdateBuilder<T> _update;
        public MongoClient MongoClient;

        /// <inheritdoc />
        public BusinessBase(IHostingEnvironment env)
        {
            BaseSettings.IsDev = env.IsDevelopment();

            _settingsDataBase = AppSettingsBase.GetSettings();

            var mongoClient = AppSettingsBase.GetMongoClient(_settingsDataBase);

            if (BaseSettings.MongoClient == null)
            {
                BaseSettings.MongoClient = mongoClient;
            }

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

        #region Connection

        /// <summary>
        ///     collection off documents
        /// </summary>
        /// <returns></returns>
        public MongoCollection<T> GetCollection()
        {
            // Create instance
            var entity = Activator.CreateInstance<T>();

            // Return instance
            return _db.GetCollection<T>(entity.CollectionName);
        }

        #endregion


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
        public void Create(IList<T> entity)
        {
            try
            {
                GetCollection().InsertBatch(entity);
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
        public T CreateReturn(T entity)
        {
            try
            {
                GetCollection().Save(entity);
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
        ///     UPDATE ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="setLastUpdate"></param>
        /// <returns></returns>
        public string UpdateOne(T entity, bool setLastUpdate = true)
        {
            try
            {
                if (setLastUpdate)
                    entity.LastUpdate = DateTimeOffset.Now.ToUnixTimeSeconds();

                GetCollection().Save(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado", ex);
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
                throw new Exception("Não Atualizado", ex);
            }
        }

        /// <summary>
        ///     UPDATE ENTITY AND RETURN
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="setLastUpdate"></param>
        /// <returns></returns>
        public T Update(T entity, bool setLastUpdate = true)
        {
            try
            {
                if (setLastUpdate)
                    entity.LastUpdate = DateTimeOffset.Now.ToUnixTimeSeconds();

                GetCollection().Save(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Não Atualizado", ex);
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

        /// <summary>
        ///     CHECK CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool CheckBy(Expression<Func<T, bool>> condition)
        {
            _query = Query<T>.Where(condition);

            return GetCollection().Find(_query).Any();
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
            return GetCollection().FindAll();
        }

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public IEnumerable<T> FindAll(IMongoSortBy sortBy)
        {
            return GetCollection().FindAll().SetSortOrder(sortBy);
        }

        /// <summary>
        ///     FIND BY CONDITION SIMPLE
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> condition)
        {
            _query = Query<T>.Where(condition);
            return GetCollection().Find(_query);
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
            return GetCollection().Find(_query).SetSortOrder(sortBy);
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
            return GetCollection().Find(_query).SetSkip(((page < 1 ? 1 : page) - 1) * limit).SetLimit(limit);
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
            return GetCollection().Find(_query).SetSortOrder(sortBy).SetSkip(((page < 1 ? 1 : page) - 1) * limit)
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
        /// <returns></returns>
        public IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, string propertyName = "Location",
            IEnumerable<IMongoQuery> queries = null)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);

            var listQueries = new List<IMongoQuery>
            {
                Query<T>.Near(x => x.GetType().GetProperty(propertyName), lat, lng, _radius)
            };

            if (queries != null)
                listQueries.AddRange(queries);

            _query = Query.And(listQueries);

            return GetCollection().Find(_query).ToList();
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
        /// <returns></returns>
        public IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, int page, int limit = 30,
            string propertyName = "Location",
            IEnumerable<IMongoQuery> queries = null)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);

            var listQueries = new List<IMongoQuery>
            {
                Query<T>.Near(x => x.GetType().GetProperty(propertyName), lat, lng, _radius)
            };

            if (queries != null)
                listQueries.AddRange(queries);


            _query = Query.And(listQueries);

            return GetCollection().Find(_query).SetSkip(((page < 1 ? 1 : page) - 1) * limit).SetLimit(limit);
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
        /// <returns></returns>
        public IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, IMongoSortBy sortBy,
            string propertyName = "Location", IEnumerable<IMongoQuery> queries = null)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);

            var listQueries = new List<IMongoQuery>
            {
                Query<T>.Near(x => x.GetType().GetProperty(propertyName), lat, lng, _radius)
            };

            if (queries != null)
                listQueries.AddRange(queries);

            _query = Query.And(listQueries);

            return GetCollection().Find(_query).SetSortOrder(sortBy).ToList();
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
        /// <returns></returns>
        public IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, IMongoSortBy sortBy, int page,
            int limit = 30,
            string propertyName = "Location", IEnumerable<IMongoQuery> queries = null)
        {
            _radius = CalculateRadius(maxDistance < 0.01 ? 1 : maxDistance);

            var listQueries = new List<IMongoQuery>
            {
                Query<T>.Near(x => x.GetType().GetProperty(propertyName), lat, lng, _radius)
            };

            if (queries != null)
                listQueries.AddRange(queries);

            _query = Query.And(listQueries);

            return GetCollection().Find(_query).SetSortOrder(sortBy).SetSkip(((page < 1 ? 1 : page) - 1) * limit)
                .SetLimit(limit);
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION AND RETURN AND RETURN DISTANCE
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="typeMetric"></param>
        /// <param name="spherical"></param>
        /// <param name="propertyIndex"></param>
        /// <param name="distanceProperty"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        public IEnumerable<T> FindByNearWithDistance(double lat, double lng, double maxDistance,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null)
        {
            var listQueries = new List<IMongoQuery>();

            if (queries != null)
                listQueries.AddRange(queries.ToList());

            double.TryParse(
                typeof(Metric).GetProperty(Enum.GetName(typeof(TypeMetric), (int)typeMetric)).GetValue(this, null)
                    .ToString(), out _earthRadius);

            if (GetCollection().IndexExists(IndexKeys.GeoSpatialSpherical(propertyIndex)) == false)
                GetCollection().CreateIndex(IndexKeys.GeoSpatialSpherical(propertyIndex));

            var stage = GenerateGeonearStage(lat, lng, listQueries, distanceProperty, maxDistance, _earthRadius, typeMetric);

            return GetCollection().Aggregate(new AggregateArgs()
            {
                Pipeline = new List<BsonDocument>() { stage },

            }).Select(x => BsonSerializer.Deserialize<T>(x)).ToList();

            var response = GetCollection().GeoNear(new GeoNearArgs
            {
                DistanceMultiplier = _earthRadius,
                MaxDistance = maxDistance / _earthRadius,
                Spherical = spherical,
                Query = listQueries.Any() ? Query.And(listQueries) : null,
                Near = new GeoNearPoint.Legacy(new XYPoint(lat, lng))
            });

            if (response == null)
                return new List<T>();

            var resultDoments = response.Hits.ToList();

            return resultDoments.Select(result =>
            {
                var item = result.Document;
                typeof(T).GetProperty(distanceProperty).SetValue(item, result.Distance, null);
                return item;
            }).ToList();
        }

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION AND RETURN AND RETURN DISTANCE
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
        /// <returns></returns>
        public IEnumerable<T> FindByNearWithDistance(double lat, double lng, double maxDistance, int page,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, int limit = 30)
        {
            var listQueries = new List<IMongoQuery>();

            if (queries != null)
                listQueries.AddRange(queries.ToList());

            double.TryParse(typeof(Metric).GetProperty(Enum.GetName(typeof(TypeMetric), (int)typeMetric)).GetValue(this, null)
                    .ToString(), out _earthRadius);

            if (GetCollection().IndexExists(IndexKeys.GeoSpatialSpherical(propertyIndex)) == false)
                GetCollection().CreateIndex(IndexKeys.GeoSpatialSpherical(propertyIndex));

            var stage = GenerateGeonearStage(lat, lng, listQueries, distanceProperty, maxDistance, _earthRadius, typeMetric);

            return GetCollection().Aggregate(new AggregateArgs()
            {
                Pipeline = new List<BsonDocument>() { stage },
            }).Select(x => BsonSerializer.Deserialize<T>(x)).ToList();
        }


        /// <summary>
        ///     COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> condition)
        {
            _query = Query<T>.Where(condition);
            return (int)GetCollection().Count(_query);
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

        /// <summary>
        ///     return radius
        /// </summary>
        /// <param name="radius">VALUE IN KM</param>
        /// <returns></returns>
        public double CalculateRadius(double radius)
        {
            return Math.Abs(radius) < 0 ? 0 : radius / 111.12;
        }

        #region Configuration

        /// <summary>
        ///     readconfig dataclient
        /// </summary>
        /// <returns></returns>
        private MongoClientSettings ReadMongoClientSettings()
        {
            MongoClientSettings mongoClientSettings = null;

            if (string.IsNullOrEmpty(_settingsDataBase.ConnectionString) == false)
            {
                mongoClientSettings = MongoClientSettings.FromConnectionString(_settingsDataBase.ConnectionString);

                return mongoClientSettings;
            }

            mongoClientSettings = new MongoClientSettings
            {
                ConnectionMode = ConnectionMode.Automatic,
                Servers = ListServers(),
                MaxConnectionPoolSize = _settingsDataBase.MaxConnections.GetValueOrDefault(),
                MinConnectionPoolSize = _settingsDataBase.MinConnections.GetValueOrDefault()

            };

            if (string.IsNullOrEmpty(_settingsDataBase.Password) || string.IsNullOrEmpty(_settingsDataBase.User))
                return mongoClientSettings;

            var mongoCredential = MongoCredential.CreatePlainCredential(_settingsDataBase.Name,
                _settingsDataBase.User, _settingsDataBase.Password);

            mongoClientSettings.Credential = mongoCredential;

            return mongoClientSettings;
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

        #endregion

        private BsonDocumentPipelineStageDefinition<T, T> GenerateGeonearStage<T>(double lat, double lng, List<IMongoQuery> listQueries, string distanceProperty, double maxDistance, double earthRadius, TypeMetric typeMetric) where T : ModelBase
        {
            try
            {
                var geoPoint = new BsonDocument
                {
                    {"type","Point"},
                    {"coordinates",new BsonArray(new double[]{ lat, lng })}
                };

                var geoNearOptions = new BsonDocument
                {
                    {"near", geoPoint},
                    {"distanceField", distanceProperty},
                    {"maxDistance",maxDistance * 1000},
                    {"query", listQueries.Count() > 0? Query.And(listQueries).ToBsonDocument() : new BsonDocument() },
                    {"spherical", true},
                };

                if (typeMetric == TypeMetric.Kilometers)
                    geoNearOptions.Add(new BsonElement("distanceMultiplier", 0.001));

                return new BsonDocumentPipelineStageDefinition<T, T>(new BsonDocument { { "$geoNear", geoNearOptions } });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private BsonDocument GenerateGeonearStage(double lat, double lng, List<IMongoQuery> listQueries, string distanceProperty, double maxDistance, double earthRadius, TypeMetric typeMetric)
        {
            try
            {
                var geoPoint = new BsonDocument
                {
                    {"type","Point"},
                    {"coordinates",new BsonArray(new double[]{ lat, lng })}
                };

                var geoNearOptions = new BsonDocument
                {
                    {"near", geoPoint},
                    {"distanceField", distanceProperty},
                    {"maxDistance",maxDistance * 1000},
                    {"query", listQueries.Count() > 0? Query.And(listQueries).ToBsonDocument() : new BsonDocument() },
                    {"spherical", true},
                };

                if (typeMetric == TypeMetric.Kilometers)
                    geoNearOptions.Add(new BsonElement("distanceMultiplier", 0.001));

                return new BsonDocument("$geoNear", geoNearOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}