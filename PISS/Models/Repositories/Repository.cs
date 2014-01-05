using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace PISS.Models.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private bool disposed = false;
        private SystemContext context { get; set; }

        public Repository()
        {
            this.context = new SystemContext();
        }

        public SystemContext Context
        {
            get
            {
                return this.context;
            }
        }

        public virtual IEnumerable<T> GetAll(int pageSize, int offset)
        {
            return context.Set<T>().Skip(offset).Take(pageSize);
        }

        public virtual IQueryable<T> GetQuery()
        {
            return context.Set<T>();
        }

        public virtual T Get(T entry)
        {
            return context.Entry<T>(entry).Entity;
        }

        public virtual T Add(T entry)
        {
            T result = context.Set<T>().Add(entry);
            return result;
        }

        public virtual T Update(T entry)
        {
            DbEntityEntry<T> entity = context.Entry(entry);
            if (entity.State == EntityState.Detached)
                this.context.Set<T>().Attach(entry);
            entity.State = EntityState.Modified;
            return entity.Entity;
        }

        public virtual void Delete(T entry)
        {
            if (context.Entry(entry).State == EntityState.Detached)
                this.context.Set<T>().Attach(entry);
            context.Set<T>().Remove(entry);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }
                disposed = true;
            }
        }

        ~Repository() { Dispose(false); }

        public void SaveChanges()
        {
            this.context.SaveChanges();
        }
    }
}