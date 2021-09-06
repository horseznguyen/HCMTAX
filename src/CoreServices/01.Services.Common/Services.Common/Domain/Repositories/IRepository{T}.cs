using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Services.Common.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        #region Refresh

        void RefreshEntity(T entity);

        #endregion Refresh

        #region Get

        IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null, bool isTracking = false, params Expression<Func<T, object>>[] includeProperties);

        #endregion Get

        #region Add

        List<T> AddRange(IEnumerable<T> entities);

        T Add(T entity);

        #endregion Add

        #region Update

        void Update(T entity, params Expression<Func<T, object>>[] changedProperties);

        void Update(T entity, params string[] changedProperties);

        void Update(T entity);

        void UpdateRange(IEnumerable<T> entities);

        #endregion Update

        #region Delete

        void DeleteRange(IEnumerable<T> entities);

        void Delete(T entity);

        void DeleteWhere(Expression<Func<T, bool>> predicate);

        #endregion Delete
    }
}