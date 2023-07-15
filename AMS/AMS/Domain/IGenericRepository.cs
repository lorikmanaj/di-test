using AMS.Domain.Models;
using System.Linq.Expressions;

namespace AMS.Domain
{
    public interface IGenericRepository<T> where T : class
    {
        Task<User> GetFirstOrDefaultAsync(Expression<Func<User, bool>> predicate);
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
    }
}
