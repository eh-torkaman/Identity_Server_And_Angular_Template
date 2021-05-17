using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly RepositoryContext repositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }
        public IQueryable<T> FindAll(bool trackChanges)
        {
            return trackChanges ? repositoryContext.Set<T>()
                : repositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            return trackChanges ? repositoryContext.Set<T>().Where(expression)
                   : repositoryContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            repositoryContext.Set<T>().Add(entity);
        }
        public void Update(T entity)
        {
            repositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            repositoryContext.Set<T>().Remove(entity);
        }

       
        ////?
        //public virtual void SaveChanges()
        //{
        //    repositoryContext.SaveChanges();
        //}
       
    }
}
