using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Timesheet.Core.Entites;

namespace Repositroy
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly TimesheetContext _dbContext;

        public DbSet<T> DbSet { get; set; }

        public BaseRepository(TimesheetContext DbContext)
        {
            DbSet = DbContext.Set<T>();
            _dbContext = DbContext;
        }

        public virtual void Delete(T entity)
        {
            DbSet.Remove(entity);
        }
       

       

        public virtual List<T> Add(List<T> entities)
        {
            if (entities != null && entities.Count() > 0)
                DbSet.AddRange(entities);
            return entities;
        }

        public T Add<T>(T entity)
        {
            _dbContext.Add(entity);
            return entity;
        }
        public async Task<T> AddAsync<T>(T entity)
        {
            await _dbContext.AddAsync(entity);
            return entity;
        }
        public T Get(Expression<Func<T, bool>> where)
        {
            return DbSet.FirstOrDefault<T>(where);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.FirstOrDefaultAsync<T>(where);
        }

        
        public virtual IQueryable<T> Where(
    Expression<Func<T, bool>>? where = null,
    int? skip = null,
    int? take = null)
        {
            IQueryable<T> query = DbSet;

            if (where != null)
                query = query.Where(where);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return query;
        }


        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }
        public void Delete<T>(T entity)
        {
            _dbContext.Remove(entity);
        }

        public T Update(T entity)
        {
            DbSet.Attach(entity);
            //_dbContext.Set<T>().AddOrUpdate(entity);            
            _dbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public List<T> Add<T>(List<T> entities)
        {
            if (entities != null && entities.Count() > 0)

                _dbContext.AddRange(entities);
            return entities;
        }
    }
}
