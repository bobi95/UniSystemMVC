using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class BaseRepository<T> where T : BaseEntity
    {
        protected readonly AppContext context;
        protected readonly DbSet<T> dbSet;
        public BaseRepository(AppContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public T GetById(int id)
        {
            return dbSet.Find(id);
        }
        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public void Insert(T item)
        {
            dbSet.Add(item);
        }
        public virtual void Delete(int id)
        {
            T entity = dbSet.Find(id);
            dbSet.Remove(entity);
        }
        public void Update(T item)
        {
            context.Entry(item).State = EntityState.Modified;
        }
        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}