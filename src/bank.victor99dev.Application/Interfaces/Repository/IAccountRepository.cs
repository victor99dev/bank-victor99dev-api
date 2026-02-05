using bank.victor99dev.Domain.Entities;

namespace bank.victor99dev.Application.Interfaces.Repository
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Account> CreateAsync(Account entity, CancellationToken cancellationToken = default);
        void Update(Account entity);
        void Remove(Account account);
    }
}