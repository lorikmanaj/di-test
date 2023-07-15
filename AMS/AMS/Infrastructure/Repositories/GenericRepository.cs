using AMS.Data;
using AMS.Domain;
using AMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AMS.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AMSDb _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AMSDb dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<User> GetFirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(predicate);
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
