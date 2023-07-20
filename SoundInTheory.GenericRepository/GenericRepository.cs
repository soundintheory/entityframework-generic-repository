using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SoundInTheory.GenericRepository
{
    public class GenericRepository<TContext, TEntity> : IGenericRepository<TContext, TEntity> where TEntity : class where TContext : DbContext, new()
    {
        protected readonly DbSet<TEntity> dbSet;

        protected readonly DbContext db;

        public GenericRepository(TContext ctx)
        {
            db = ctx;
            dbSet = db.Set<TEntity>();
        }

        public virtual TEntity Find(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            var result = dbSet.Find(id);
            return result;
        }

        public virtual IEnumerable<TEntity> FindWithTracking(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string[] includeProperties = null)
        {
            return FindAsQueryable(filter, orderBy, includeProperties).ToList();
        }

        public virtual IEnumerable<TEntity> Find(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string[] includeProperties = null)
        {
            return FindAsQueryable(filter, orderBy, includeProperties).AsNoTracking().ToList();
        }

        public virtual IEnumerable<TEntity> Find(
           Expression<Func<TEntity, bool>> filter = null,
           params string[] includeProperties)
        {
            return FindAsQueryable(filter, null, includeProperties).AsNoTracking().ToList();
        }

        public virtual TEntity FindOne(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string[] includeProperties = null)
        {
            return FindAsQueryable(filter, orderBy, includeProperties).FirstOrDefault();
        }

        public virtual TEntity FindOne(
           Expression<Func<TEntity, bool>> filter = null,
           params string[] includeProperties)
        {
            return FindAsQueryable(filter, null, includeProperties).FirstOrDefault();
        }

        public virtual IQueryable<TEntity> FindAsQueryable(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string[] includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return orderBy != null ? orderBy(query) : query;
        }

        public virtual IQueryable<TEntity> FindAsQueryable(
           Expression<Func<TEntity, bool>> filter = null,
           params string[] includeProperties)
        {
            return FindAsQueryable(filter, null, includeProperties);
        }

        public IEnumerable<TEntity> All()
        {
            return dbSet.AsNoTracking().ToList();
        }

        public virtual TEntity Create(TEntity entity)
        {
            dbSet.Add(entity);
            db.SaveChanges();
            return entity;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            dbSet.Add(entity);
            await db.SaveChangesAsync();
            return entity;
        }

        public virtual IEnumerable<TEntity> CreateMany(IEnumerable<TEntity> entities)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var entity in entities)
                    {
                        dbSet.Add(entity);
                    }

                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
            return entities;
        }

        public virtual TEntity Update(TEntity entityToUpdate)
        {
            Attach(entityToUpdate);
            db.Entry(entityToUpdate).State = EntityState.Modified;
            db.SaveChanges();
            return entityToUpdate;
        }

        public virtual IEnumerable<TEntity> Take(int count, params string[] includeProperties)
        {
            var query = dbSet.Take(count);
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return query;
        }

        public virtual IEnumerable<TEntity> TakeLast(int count, params string[] includeProperties)
        {
            var query = dbSet.OrderByDescending(x => x).Take(count);
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return query;
        }

        public virtual IEnumerable<TEntity> UpdateMany(IEnumerable<TEntity> entities)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var entity in entities)
                    {
                        Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
            return entities;
        }

        public virtual bool Delete(int? id)
        {
            if (!id.HasValue)
            {
                return false;
            }

            var entityToDelete = dbSet.Find(id);
            return Delete(entityToDelete);
        }

        public virtual bool Delete(TEntity entityToDelete)
        {
            try
            {
                Attach(entityToDelete);
                dbSet.Remove(entityToDelete);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool DeleteRange(Expression<Func<TEntity, bool>> selectionToDelete)
        {
            if (selectionToDelete == null)
            {
                return false;
            }

            var entitiesToDelete = FindWithTracking(selectionToDelete);
            DeleteRange(entitiesToDelete);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Deletes a range
        /// </summary>
        /// <param name="collectionToDelete">Items must be "attached"</param>
        /// <returns></returns>
        public virtual bool DeleteRange(IEnumerable<TEntity> collectionToDelete)
        {
            dbSet.RemoveRange(collectionToDelete);
            db.SaveChanges();
            return true;
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        protected void Attach(TEntity entity)
        {
            if (!dbSet.Local.Any(e => e == entity) || db.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
        }

        public bool Validate(object entity, List<ValidationResult> results)
        {
            var context = new ValidationContext(entity);

            return Validator.TryValidateObject(entity, context, results);
        }

        public bool IsValid(object entity)
        {
            return Validate(entity, new List<ValidationResult>());
        }

        public bool Any(Expression<Func<TEntity, bool>> filter = null)
        {
            return FindAsQueryable(filter).Any();
        }

        #region IDisposable Support

        private bool _isDisposed = false;

        public virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed && isDisposing)
            {
                db.Dispose();
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#if NETSTANDARD2_0_OR_GREATER
        public int Query(string sql, params object[] parameters)
        {
            return db.Database.ExecuteSqlCommand(sql, parameters);
        }
        public List<TEntity> Query<TResult>(string sql, string includeProperties, params object[] parameters)
        {
            var query = dbSet.FromSql(sql, parameters);
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return query.ToList();
        }
#endif

#if NET6_0_OR_GREATER
        public int Query(string sql, params object[] parameters)
        {
            return db.Database.ExecuteSqlRaw(sql, parameters);
        }
        public List<TEntity> Query<TResult>(string sql, string includeProperties, params object[] parameters)
        {
            var query = dbSet.FromSqlRaw(sql, parameters);
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return query.ToList();
        }
#endif

        public List<TEntity> Query<TResult>(string sql, params object[] parameters)
        {
            return Query<TResult>(sql, null, parameters);
        }

#endregion IDisposable Support
    }
}
