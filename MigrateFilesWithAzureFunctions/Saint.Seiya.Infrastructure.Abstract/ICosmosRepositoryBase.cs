using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Saint.Seiya.Infrastructure.Abstract
{
    public interface ICosmosRepositoryBase<T>
    {
        Task<T> AddOrUpdateAsync(T entity);
        int CountAll();
        int CountAll(Expression<Func<T, bool>> predicate);
        Task<bool> DeleteAsync(string itemId, string partitionKey);
        Task<T> ExecuteStoredProcedureAsync(string storedProcedure, string partitioKey, dynamic[] procedureParams);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending, int top);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending, int skip, int take);
        Task<T> GetByIdAsync(string itemId, string partitionKey);
    }
}
