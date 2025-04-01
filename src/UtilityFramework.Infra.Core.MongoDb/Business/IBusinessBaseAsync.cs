using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace UtilityFramework.Infra.Core.MongoDb.Business
{
    public interface IBusinessBaseAsync<T> where T : class
    {
        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<string> CreateAsync(T entity);

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> CreateReturnAsync(T entity);

        /// <summary>
        ///     REGISTER A LIST ENTITY
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<IList<T>> CreateReturnAsync(IList<T> entities);

        /// <summary>
        ///     CREATE MULTIPLE ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateAsync(IList<T> entity);

        /// <summary>
        ///     UPDATE ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="setLastUpdate"></param>
        /// <returns></returns>
        Task<string> UpdateOneAsync(T entity, bool setLastUpdate = true);

        /// <summary>
        ///     UPDATE ENTITY AND RETURN
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="setLastUpdate"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity, bool setLastUpdate = true);

        /// <summary>
        ///     DELETE FROM ID
        /// </summary>
        /// <param name="id"></param>
        Task<long> DeleteOneAsync(string id);

        /// <summary>
        ///     DELETE ALL FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        Task<long> DeleteAsync(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     CHECK CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<bool> CheckByAsync(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DisableOneAsync(string id);

        /// <summary>
        ///     SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> EnableOneAsync(string id);

        /// <summary>
        ///     FIND ENDITY FROM ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> FindByIdAsync(string id);

        /// <summary>
        ///     FIND ONE ENTITY FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<T> FindOneByAsync(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     FIND ALL ENTITY
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> FindAllAsync();

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindAllAsync(SortDefinition<T> sortBy);

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindAllAsync(SortDefinition<T> sortBy, int page, int limit = 30);

        /// <summary>
        ///     FIND BY CONDITION SIMPLE
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     FIND BY CONDITION WITH ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> condition, SortDefinition<T> sortBy);

        /// <summary>
        ///     FINDY BY CONDITION WITH PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> condition, int page, int limit = 30);

        /// <summary>
        ///     FIND BY CONDITION WITH SORTBY AND PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> condition, int page, SortDefinition<T> sortBy,
            int limit = 30);

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
        Task<IEnumerable<T>> FindByNearAsync(double lat, double lng, double maxDistance,
            string propertyName = "Location", IEnumerable<FilterDefinition<T>> queries = null, double minDistance = 0, bool isLngLat = false);

        /// <summary>
        ///     FIND BY NEAR WITH CONDITIONS and pagination
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
        Task<IEnumerable<T>> FindByNearAsync(double lat, double lng, double maxDistance, int page, int limit = 30,
            string propertyName = "Location", IEnumerable<FilterDefinition<T>> queries = null, double minDistance = 0, bool isLngLat = false);

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
        Task<IEnumerable<T>> FindByNearAsync(double lat, double lng, double maxDistance, SortDefinition<T> sortBy,
            string propertyName = "Location", IEnumerable<FilterDefinition<T>> queries = null, double minDistance = 0, bool isLngLat = false);

        /// <summary>
        ///     FIND BY NEAR WITH CONDITIONS AND SORTBY
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="limit"></param>
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByNearAsync(double lat, double lng, double maxDistance, SortDefinition<T> sortBy,
            int page, int limit = 30, string propertyName = "Location", IEnumerable<FilterDefinition<T>> queries = null,
            double minDistance = 0, bool isLngLat = false);

        /// <summary>
        ///     COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<long> CountLongAsync(Expression<Func<T, bool>> condition);

        Task<IEnumerable<T>> LoadDataTableAsync(string searchTerm, SortDefinition<T> sortBy, int skip, int limit = 10,
             params string[] fields);

        Task<IEnumerable<T>> LoadDataTableAsync(string searchTerm, SortDefinition<T> sortBy, int skip, int limit = 10,
            IEnumerable<FilterDefinition<T>> queries = null, params string[] fields);

        Task<IEnumerable<T>> LoadDataTableAsync(Expression<Func<T, bool>> condition, string searchTerm,
            SortDefinition<T> sortBy, int skip, int limit = 10, params string[] fields);

        Task<long> CountSearchDataTableAsync(string searchTerm, params string[] fields);

        Task<long> CountSearchDataTableAsync(string searchTerm, IEnumerable<FilterDefinition<T>> queries = null,
            params string[] fields);

        Task<long> CountSearchDataTableAsync(Expression<Func<T, bool>> condition, string searchTerm,
            params string[] fields);

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByContais(string paramName, string value);

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByContais(Expression<Func<T, bool>> condition, string paramName, string value);

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByContais(Expression<Func<T, bool>> condition, string paramName, string value,
            SortDefinition<T> sortBy);

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByContais(Expression<Func<T, bool>> condition, string paramName, string value,
            SortDefinition<T> sortBy, int limit = 30);

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
        Task<IEnumerable<T>> FindByContais(Expression<Func<T, bool>> condition, string paramName, string value,
            SortDefinition<T> sortBy, int page, int limit = 30);

        /// <summary>
        ///     FIND ALL IN
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindIn<TY>(string paramName, List<TY> value);

        /// <summary>
        ///     FIND IN
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindIn<TY>(Expression<Func<T, bool>> condition, string paramName, List<TY> value,
            SortDefinition<T> sortBy);

        /// <summary>
        ///     FIND IN
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindIn<TY>(Expression<Func<T, bool>> condition, string paramName, List<TY> value,
            SortDefinition<T> sortBy, int limit = 30);

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
        Task<IEnumerable<T>> FindIn<TY>(Expression<Func<T, bool>> condition, string paramName, List<TY> value,
            SortDefinition<T> sortBy, int page, int limit = 30);

        /// <summary>
        ///     <summary>
        ///         FIND BY NEAR WITH CONDITION AND RETURN AND RETURN DISTANCE
        ///     </summary>
        ///     <param name="lat"></param>
        ///     <param name="lng"></param>
        ///     <param name="maxDistance"></param>
        ///     <param name="typeMetric"></param>
        ///     <param name="spherical"></param>
        ///     <param name="propertyIndex"></param>
        ///     <param name="distanceProperty"></param>
        ///     <param name="queries"></param>
        ///     <returns></returns>
        Task<List<T>> FindByNearWithDistanceAsync(double lat, double lng, double maxDistance,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, double? minDistance = null, bool isLngLat = false);

        /// <summary>
        ///     <summary>
        ///         FIND BY NEAR WITH CONDITION AND RETURN AND RETURN DISTANCE
        ///     </summary>
        ///     <param name="lat"></param>
        ///     <param name="lng"></param>
        ///     <param name="maxDistance"></param>
        ///     <param name="typeMetric"></param>
        ///     <param name="spherical"></param>
        ///     <param name="propertyIndex"></param>
        ///     <param name="distanceProperty"></param>
        ///     <param name="queries"></param>
        ///     <returns></returns>
        Task<List<T>> FindByNearWithDistanceAsync(double lat, double lng, double maxDistance, SortDefinition<T> sortBy,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, double? minDistance = null, bool isLngLat = false);

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
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        Task<List<T>> FindByNearWithDistanceAsync(double lat, double lng, double maxDistance, int page,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, int limit = 30, double? minDistance = null, bool isLngLat = false);

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION AND RETURN AND RETURN DISTANCE
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="typeMetric"></param>
        /// <param name="spherical"></param>
        /// <param name="propertyIndex"></param>
        /// <param name="distanceProperty"></param>
        /// <param name="queries"></param>
        /// <param name="limit"></param>
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        Task<List<T>> FindByNearWithDistanceAsync(double lat, double lng, double maxDistance, int page, SortDefinition<T> sortBy,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, int limit = 30, double? minDistance = null, bool isLngLat = false);

        /// <summary>
        ///     COLLECTION DB
        /// </summary>
        /// <returns></returns>
        IMongoCollection<T> GetCollectionAsync();
        /// <summary>
        ///  GERAR QUERY EM JSON
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string PrintQuery(FilterDefinition<T> filter);

        #region Sync Methods

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string Create(T entity);

        /// <summary>
        ///     REGISTER ENTITY MULTIPLE
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Create(IList<T> entity);

        /// <summary>
        ///     REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T CreateReturn(T entity);

        /// <summary>
        ///     UPDATE ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string UpdateOne(T entity);

        /// <summary>
        ///     UPDATE ENTITY MULTIPLE
        /// </summary>
        /// <param name="update"></param>
        /// <param name="flags"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        void UpdateMultiple(IMongoQuery condition, UpdateBuilder<T> update, UpdateFlags flags);

        /// <summary>
        ///     UPDATE ENTITY AND RETURN
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Update(T entity);

        /// <summary>
        ///     DELETE FROM ID
        /// </summary>
        /// <param name="id"></param>
        void DeleteOne(string id);

        /// <summary>
        ///     DELETE ALL FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        void Delete(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     DELETE FROM ENTITY
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        ///     CHECK CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool CheckBy(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DisableOne(string id);

        /// <summary>
        ///     SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool EnableOne(string id);

        /// <summary>
        ///     FIND ENDITY FROM ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T FindById(string id);

        /// <summary>
        ///     FIND ONE ENTITY FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        T FindOneBy(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     FIND ALL ENTITY
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> FindAll();

        /// <summary>
        ///     FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        IEnumerable<T> FindAll(IMongoSortBy sortBy);

        /// <summary>
        ///     FIND BY CONDITION SIMPLE
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     FIND BY CONDITION WITH ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, IMongoSortBy sortBy);

        /// <summary>
        ///     FINDY BY CONDITION WITH PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, int page, int limit = 30);

        /// <summary>
        ///     FIND BY CONDITION WITH SORTBY AND PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, int page, IMongoSortBy sortBy, int limit = 30);

        /// <summary>
        ///     FIND BY NEAR WITH CONDITION
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="isLngLat"></param>
        /// <param name="minDistance"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, string propertyName = "Location",
            IEnumerable<IMongoQuery> queries = null, bool isLngLat = false);

        /// <summary>
        ///     FIND BY NEAR WITH CONDITIONS and pagination
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="isLngLat"></param>
        /// <param name="minDistance"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, int page, int limit = 30,
            string propertyName = "Location", IEnumerable<IMongoQuery> queries = null, bool isLngLat = false);

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
        IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, IMongoSortBy sortBy,
            string propertyName = "Location", IEnumerable<IMongoQuery> queries = null, bool isLngLat = false);

        /// <summary>
        ///     FIND BY NEAR WITH CONDITIONS AND SORTBY
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="isLngLat"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, IMongoSortBy sortBy, int page,
            int limit = 30, string propertyName = "Location", IEnumerable<IMongoQuery> queries = null, bool isLngLat = false);

        /// <summary>
        ///     <summary>
        ///         FIND BY NEAR WITH CONDITION AND RETURN AND RETURN DISTANCE
        ///     </summary>
        ///     <param name="lat"></param>
        ///     <param name="lng"></param>
        ///     <param name="maxDistance"></param>
        ///     <param name="typeMetric"></param>
        ///     <param name="spherical"></param>
        ///     <param name="propertyIndex"></param>
        ///     <param name="distanceProperty"></param>
        ///     <param name="queries"></param>
        ///     <returns></returns>
        IEnumerable<T> FindByNearWithDistance(double lat, double lng, double maxDistance,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, double? minDistance = null, bool isLngLat = false);

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
        /// <param name="minDistance"></param>
        /// <param name="isLngLat"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNearWithDistance(double lat, double lng, double maxDistance, int page,
            TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location",
            string distanceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, int limit = 30, double? minDistance = null, bool isLngLat = false);

        /// <summary>
        ///     COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> condition);

        long CountLong(Expression<Func<T, bool>> condition);

        IEnumerable<T> LoadDataTable(string searchTerm, IMongoSortBy sortby, int skip, int limit = 10,
            params string[] fields);

        IEnumerable<T> LoadDataTable(string searchTerm, IMongoSortBy sortby, int skip, int limit = 10,
            IEnumerable<IMongoQuery> queries = null, params string[] fields);

        IEnumerable<T> LoadDataTable(Expression<Func<T, bool>> condition, string searchTerm, IMongoSortBy sortby,
            int skip, int limit = 10, params string[] fields);

        long CountSearchDataTable(string searchTerm, params string[] fields);

        long CountSearchDataTable(string searchTerm, IEnumerable<IMongoQuery> queries = null, params string[] fields);

        long CountSearchDataTable(Expression<Func<T, bool>> condition, string searchTerm, params string[] fields);

        #endregion
    }
}