using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PISS.Models.Repositories
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetAll(int pageSize, int offset);
        IQueryable<T> GetQuery();
        T Get(T entry);
        T Add(T entry);
        T Update(T entry);
        void Delete(T entry);
        void SaveChanges();
    }
}
