using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace UtilityFramework.Infra.Core.MongoDb.Business
{
    public interface IBusinessBase<T> where T : class
    {
        /// <summary>
        /// REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string Create(T entity);

        /// <summary>
        /// REGISTER ENTITY MULTIPLE
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Create(IList<T> entity);

        /// <summary>
        /// REGISTER ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T CreateReturn(T entity);
        /// <summary>
        /// UPDATE ENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="setLastUpdate"></param>
        /// <returns></returns>
        string UpdateOne(T entity, bool setLastUpdate = true);

        /// <summary>
        /// UPDATE ENTITY MULTIPLE
        /// </summary>
        /// <param name="update"></param>
        /// <param name="flags"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        void UpdateMultiple(IMongoQuery condition, UpdateBuilder<T> update, UpdateFlags flags);
        /// <summary>
        /// UPDATE ENTITY AND RETURN
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="setLastUpdate"></param>
        /// <returns></returns>
        T Update(T entity, bool setLastUpdate = true);
        /// <summary>
        /// DELETE FROM ID
        /// </summary>
        /// <param name="id"></param>
        void DeleteOne(string id);
        /// <summary>
        /// DELETE ALL FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        void Delete(Expression<Func<T, bool>> condition);
        /// <summary>
        /// CHECK CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool CheckBy(Expression<Func<T, bool>> condition);
        /// <summary>
        ///  SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DisableOne(string id);
        /// <summary>
        ///  SET DISABLED
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool EnableOne(string id);
        /// <summary>
        /// FIND ENDITY FROM ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T FindById(string id);
        /// <summary>
        /// FIND ONE ENTITY FROM CONDITION
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        T FindOneBy(Expression<Func<T, bool>> condition);
        /// <summary>
        /// FIND ALL ENTITY 
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> FindAll();
        /// <summary>
        /// FIND ALL ENTITY ORDERBY
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        IEnumerable<T> FindAll(IMongoSortBy sortBy);
        /// <summary>
        /// FIND BY CONDITION SIMPLE
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> condition);
        /// <summary>
        /// FIND BY CONDITION WITH ORDERBY
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, IMongoSortBy sortBy);
        /// <summary>
        /// FINDY BY CONDITION WITH PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, int page, int limit = 30);
        /// <summary>
        /// FIND BY CONDITION WITH SORTBY AND PAGINATION
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> condition, int page, IMongoSortBy sortBy, int limit = 30);
        /// <summary>
        /// FIND BY NEAR WITH CONDITION
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, string propertyName = "Location", IEnumerable<IMongoQuery> queries = null);

        /// <summary>
        /// FIND BY NEAR WITH CONDITIONS and pagination
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, int page, int limit = 30, string propertyName = "Location", IEnumerable<IMongoQuery> queries = null);
        /// <summary>
        /// FIND BY NEAR WITH CONDITIONS AND SORTBY
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="sortBy"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, IMongoSortBy sortBy, string propertyName = "Location", IEnumerable<IMongoQuery> queries = null);

        /// <summary>
        /// FIND BY NEAR WITH CONDITIONS AND SORTBY
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="propertyName"></param>
        /// <param name="queries"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNear(double lat, double lng, double maxDistance, IMongoSortBy sortBy, int page, int limit = 30, string propertyName = "Location", IEnumerable<IMongoQuery> queries = null);        /// <summary>

        /// <summary>
        /// FIND BY NEAR WITH CONDITION AND RETURN AND RETURN DISTANCE
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="typeMetric"></param>
        /// <param name="spherical"></param>
        /// <param name="propertyIndex"></param>
        /// <param name="distaceProperty"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        IEnumerable<T> FindByNearWithDistance(double lat, double lng, double maxDistance, TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location", string distaceProperty = "Distance", IEnumerable<IMongoQuery> queries = null);
        /// <summary>
        /// FIND BY NEAR WITH CONDITION AND RETURN AND RETURN DISTANCE
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="maxDistance"></param>
        /// <param name="page"></param>
        /// <param name="typeMetric"></param>
        /// <param name="spherical"></param>
        /// <param name="propertyIndex"></param>
        /// <param name="distaceProperty"></param>
        /// <param name="queries"></param>
        /// <param name="limit"></param>
        /// <returns></returns>

        IEnumerable<T> FindByNearWithDistance(double lat, double lng, double maxDistance, int page, TypeMetric typeMetric = TypeMetric.Kilometers, bool spherical = true, string propertyIndex = "Location", string distaceProperty = "Distance", IEnumerable<IMongoQuery> queries = null, int limit = 30);
        /// <summary>
        /// COUNT ENTITYS FROM CONDITIONS
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> condition);
        long CountLong(Expression<Func<T, bool>> condition);
        /// <summary>
        /// COLLECTION DB
        /// </summary>
        /// <returns></returns>
        MongoCollection<T> GetCollection();
    }
}