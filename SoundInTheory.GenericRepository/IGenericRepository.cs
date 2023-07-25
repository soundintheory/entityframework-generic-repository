using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SoundInTheory.GenericRepository
{
    public interface IGenericRepository<TContext, TEntity> : IDisposable where TEntity : class where TContext : DbContext
    {
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string[] includeProperties = null);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter = null, params string[] includeProperties);
        TEntity FindOne(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string[] includeProperties = null);
        TEntity FindOne(Expression<Func<TEntity, bool>> filter = null, params string[] includeProperties);
        TEntity Find(int? id, bool withTracking = false);
        IQueryable<TEntity> FindAsQueryable(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string[] includeProperties = null);
        IQueryable<TEntity> FindAsQueryable(Expression<Func<TEntity, bool>> filter = null, params string[] includeProperties);
        TEntity Create(TEntity entity);
        IEnumerable<TEntity> CreateMany(IEnumerable<TEntity> entities);
        TEntity Update(TEntity entityToUpdate);
        IEnumerable<TEntity> UpdateMany(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> Take(int count, params string[] includeProperties);
        IEnumerable<TEntity> TakeLast(int count, params string[] includeProperties);
        bool Delete(int? id);
        bool Delete(TEntity entityToDelete);
        IEnumerable<TEntity> All();
        int SaveChanges();
        bool Validate(object entity, List<ValidationResult> results);
        bool IsValid(object entity);
        bool Any(Expression<Func<TEntity, bool>> filter = null);

        int Query(string sql, params object[] parameters);

        List<TEntity> Query<TResult>(string sql, string includeProperties, params object[] parameters);

        List<TEntity> Query<TResult>(string sql, params object[] parameters);
    }
}
