using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositroy
{
    public interface IRepository<T> where T : class
    {
        T Add<T>(T entity);
        Task<T> AddAsync<T>(T entity);
        T Update(T entity);
        void Delete<T>(T entity);
        T Get(Expression<Func<T, bool>> where);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        void Delete(T entity);
        List<T> Add(List<T> entities);

        Task<T> GetAsync(Expression<Func<T, bool>> where);
        IQueryable<T> Where(Expression<Func<T, bool>>? where = null,int? skip = null,int? take = null);
    }
}
