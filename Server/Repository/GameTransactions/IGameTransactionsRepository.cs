using Domain.Entities;

namespace Repository.GameTransactions;

public interface IGameTransactionsRepository
{
    public Task<IReadOnlyCollection<GameTransactionsRecord>> GetAllAsync();

    public Task<GameTransactionsRecord?> GetByIdAsync(Guid id);

    public Task<Guid> AddAsync(GameTransactionsRecord data);

    public void Update(GameTransactionsRecord data);

    public Task<bool> DeleteAsync(Guid id);

    public Task<int> SaveChangesAsync();
}