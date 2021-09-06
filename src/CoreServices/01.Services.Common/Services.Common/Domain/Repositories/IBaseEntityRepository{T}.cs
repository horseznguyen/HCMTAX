using Services.Common.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Services.Common.Domain.Repositories
{
    public interface IBaseEntityRepository<TEntity> where TEntity : BaseEntity
    {
        #region Refresh

        void RefreshEntity(TEntity entity);

        #endregion Refresh

        #region Get

        IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties);

        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null,
            bool isIncludeDeleted = false,
            bool isTracking = false,
            params Expression<Func<TEntity, object>>[] includeProperties);

        #endregion Get

        #region Add

        TEntity Add(TEntity entity);

        List<TEntity> AddRange(IEnumerable<TEntity> entities);

        #endregion Add

        #region Update

        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] changedProperties);

        void Update(TEntity entity, params string[] changedProperties);

        void Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        #endregion Update

        #region Delete

        void Delete(TEntity entity, bool isPhysicalDelete = false);

        void DeleteRange(IEnumerable<TEntity> entities, bool isPhysicalDelete = false);

        void DeleteWhere(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false);

        #endregion Delete
    }
}