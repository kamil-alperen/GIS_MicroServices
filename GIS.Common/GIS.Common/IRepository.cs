
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace GIS.Common
{
    public interface IRepository<T>
    {
        Task<Result<bool>> DeleteAsync(Guid id);
        Task<Result<IReadOnlyCollection<T>>> GetAllAsync();
        Task<Result<IReadOnlyCollection<T>>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<Result<T>> GetAsync(Guid id);
        Task<Result<T>> GetAsync(Expression<Func<T, bool>> filter);
        Task<Result<T>> PostAsync(T countryEntity);
        Task<Result<bool>> PutAsync(T countryEntity);
    }
}