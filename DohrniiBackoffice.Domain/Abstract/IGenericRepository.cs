using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DohrniiBackoffice.Domain.Abstract
{
    public interface IGenericRepository<T>
    where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void Dispose();
        void Edit(T entity);
        IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();

        //bool Save();
        Task<bool> Save(string username, string Ip);
    }
}
