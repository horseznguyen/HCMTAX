using Microsoft.EntityFrameworkCore;
using Services.Common.Domain.Entities;
using Services.Common.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFCore.Common.Repositories
{
    public abstract class BaseEntityRepository<T> : Repository<T>, IBaseEntityRepository<T> where T : BaseEntity
    {
        protected BaseEntityRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region Get

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate = null,
            bool isIncludeDeleted = false,
            bool isTracking = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = DbSet.AsQueryable();

            if (isTracking) query = query.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            includeProperties = includeProperties?.Distinct().ToArray();

            if (includeProperties?.Any() == true)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            // NOTE: Query Filter (query.IgnoreQueryFilters()), it affect to load data business logic.
            // Currently not flexible, please check https://github.com/aspnet/EntityFrameworkCore/issues/8576
            query = isIncludeDeleted ? query.IgnoreQueryFilters() : query.Where(x => x.IsDelete == false || x.IsDelete == null);

            return query;
        }

        #endregion Get

        #region Add

        public override T Add(T entity)
        {
            entity = DbSet.Add(entity).Entity;

            return entity;
        }

        public override List<T> AddRange(IEnumerable<T> entities)
        {
            List<T> listAddedEntity = new List<T>();

            foreach (var entity in entities)
            {
                var addedEntity = Add(entity);

                listAddedEntity.Add(addedEntity);
            }

            return listAddedEntity;
        }

        #endregion Add

        #region Update

        public override void Update(T entity, params Expression<Func<T, object>>[] changedProperties)
        {
            TryAttach(entity);

            changedProperties = changedProperties?.Distinct().ToArray();

            if (changedProperties?.Any() == true)
            {
                DbContext.Entry(entity).Property(x => x.UpdateDate).IsModified = true;

                foreach (var property in changedProperties)
                {
                    DbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
            {
                DbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public override void Update(T entity, params string[] changedProperties)
        {
            TryAttach(entity);

            changedProperties = changedProperties?.Distinct().ToArray();

            if (changedProperties?.Any() == true)
            {
                DbContext.Entry(entity).Property(x => x.UpdateDate).IsModified = true;

                foreach (var property in changedProperties)
                {
                    DbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
            {
                DbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public override void Update(T entity)
        {
            TryAttach(entity);

            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public override void UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        #endregion Update

        #region Delete

        public virtual void Delete(T entity, bool isPhysicalDelete = false)
        {
            try
            {
                TryAttach(entity);

                if (!isPhysicalDelete)
                {
                    //entity.SetIsDelete(true);

                    DbContext.Entry(entity).Property(x => x.DeletionDate).IsModified = true;

                    DbContext.Entry(entity).Property(x => x.IsDelete).IsModified = true;
                }
                else
                {
                    DbSet.Remove(entity);
                }
            }
            catch (Exception)
            {
                RefreshEntity(entity);

                throw;
            }
        }

        public virtual void DeleteRange(IEnumerable<T> entities, bool isPhysicalDelete = false)
        {
            foreach (var entity in entities)
            {
                Delete(entity, isPhysicalDelete);
            }
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate, bool isPhysicalDelete = false)
        {
            var entities = Get(predicate, isPhysicalDelete).AsEnumerable();

            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    Delete(entity, isPhysicalDelete);
                }
            }
        }

        #endregion Delete
    }
}