using FCG_Users.Domain.Shared;

namespace FCG_Users.Application.Shared.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateAsync(T entidade, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entidade, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
