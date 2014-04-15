using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;

namespace MezaIT.NHibernate
{
    /// <summary>
    /// Sort direction
    /// </summary>
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// The base that most NHibernate data access objects will inherit from
    /// </summary>
    /// <typeparam name="T">Type of the entity</typeparam>
    /// <typeparam name="TID">Type of the unique identifier</typeparam>
    public abstract class NHibernateBase<T, TID> where T : class
    {
        #region Protected Properties

        /// <summary>
        /// Current NHibernate session
        /// </summary>
        protected ISession Session
        {
            get { return NHibernateSessionManager.Session; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Delete a particular entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public void Delete(T entity)
        {
            Session.Delete(entity);
        }

        /// <summary>
        /// Retrieve a particular entity
        /// </summary>
        /// <param name="id">Unique identifier of the entity</param>
        /// <returns>An entity</returns>
        public T Get(TID id)
        {
            return Session.Get<T>(id);
        }

        /// <summary>
        /// Retrieve a particular entity
        /// </summary>
        /// <param name="criteria">Criteria lambda expression</param>
        /// <returns>An entity</returns>
        public T Get(Expression<Func<T, bool>> criteria)
        {
            return Session.QueryOver<T>()
                          .Where(criteria)
                          .Cacheable()
                          .SingleOrDefault();
        }

        /// <summary>
        /// Retrieve the number of entities
        /// </summary>
        /// <returns>Number of entities</returns>
        public int GetCount()
        {
            return GetCount(null);
        }

        /// <summary>
        /// Retrieve the number of entities that matches the criteria
        /// </summary>
        /// <param name="criteria">Criteria lambda expression</param>
        /// <returns>Number of entities</returns>
        public int GetCount(Expression<Func<T, bool>> criteria)
        {
            var queryOver = Session.QueryOver<T>();

            if (criteria != null)
                queryOver = queryOver.Where(criteria);

            return queryOver
                .Cacheable()
                .RowCount();
        }

        /// <summary>
        /// Retrieve a list of entities
        /// </summary>
        /// <returns>List of entities</returns>
        public IList<T> GetList()
        {
            return GetList((Expression<Func<T, bool>>) null);
        }

        /// <summary>
        /// Retrieve a list of entities
        /// </summary>
        /// <param name="criteria">Criteria lambda expression</param>
        /// <returns>List of entities</returns>
        public IList<T> GetList(Expression<Func<T, bool>> criteria)
        {
            return GetList(criteria, null, -1, -1);
        }

        /// <summary>
        /// Retrieve a list of entities
        /// </summary>
        /// <param name="criteria">Criteria lambda expression</param>
        /// <param name="startRowIndex">Start row index</param>
        /// <param name="maximumRows">Maximum number of rows</param>
        /// <returns>List of entities</returns>
        public IList<T> GetList(Expression<Func<T, bool>> criteria, int startRowIndex, int maximumRows)
        {
            return GetList(criteria, null, startRowIndex, maximumRows);
        }

        /// <summary>
        /// Retrieve a list of entities
        /// </summary>
        /// <param name="criteria">Criteria lambda expression</param>
        /// <param name="sortProperties">Sort property dictionary</param>
        /// <param name="startRowIndex">Start row index</param>
        /// <param name="maximumRows">Maximum number of rows</param>
        /// <returns>List of entities</returns>
        public IList<T> GetList(Expression<Func<T, bool>> criteria, IDictionary<string, SortDirection> sortProperties,
                                int startRowIndex, int maximumRows)
        {
            var queryOver = Session.QueryOver<T>();

            if (criteria != null)
                queryOver = queryOver.Where(criteria);

            if (sortProperties != null)
                foreach (var sortProperty in sortProperties)
                    queryOver.UnderlyingCriteria.AddOrder(sortProperty.Value == SortDirection.Ascending
                                                              ? Order.Asc(sortProperty.Key)
                                                              : Order.Desc(sortProperty.Key));

            if (startRowIndex >= 0)
                queryOver.UnderlyingCriteria.SetFirstResult(startRowIndex);

            if (maximumRows >= 0)
                queryOver.UnderlyingCriteria.SetMaxResults(maximumRows);

            return queryOver
                .Cacheable()
                .List();
        }

        /// <summary>
        /// Retrieve a sorted list of entities
        /// </summary>
        /// <param name="sortProperties">Sort property dictionary</param>
        /// <returns>List of entities</returns>
        public IList<T> GetList(IDictionary<string, SortDirection> sortProperties)
        {
            return GetList(sortProperties, -1, -1);
        }

        /// <summary>
        /// Retrieve a paged list of entities
        /// </summary>
        /// <param name="startRowIndex">Start row index</param>
        /// <param name="maximumRows">Maximum number of rows</param>
        /// <returns>List of entities</returns>
        public IList<T> GetList(int startRowIndex, int maximumRows)
        {
            return GetList((Expression<Func<T, bool>>) null, startRowIndex, maximumRows);
        }

        /// <summary>
        /// Retrieve a sorted and paged list of entities
        /// </summary>
        /// <param name="sortProperties">Sort property dictionary</param>
        /// <param name="startRowIndex">Start row index</param>
        /// <param name="maximumRows">Maximum number of rows</param>
        /// <returns>List of entities</returns>
        public IList<T> GetList(IDictionary<string, SortDirection> sortProperties, int startRowIndex,
                                int maximumRows)
        {
            return GetList(null, sortProperties, startRowIndex, maximumRows);
        }

        /// <summary>
        /// Insert a new entity or update an existing one
        /// </summary>
        /// <param name="entity">Entity to insert or update</param>
        /// <returns>Latest version of the entity</returns>
        public T SaveOrUpdate(T entity)
        {
            Session.SaveOrUpdate(entity);

            return entity;
        }

        #endregion
    }

    /// <summary>
    /// Default implementation of <see cref="NHibernateBase{T, TID}" /> will use an integer as the unique identifier
    /// </summary>
    /// <typeparam name="T">Type of the object</typeparam>
    public abstract class NHibernateBase<T> : NHibernateBase<T, int> where T : class
    {
    }
}