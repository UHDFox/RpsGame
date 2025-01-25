using Domain.Entities;

namespace Repository.MatchHistory;

public interface IMatchHistoryRepository
{
    public Task<IReadOnlyCollection<MatchHistoryRecord>> GetAllAsync();

    public Task<MatchHistoryRecord?> GetByIdAsync(Guid id);

    public Task<Guid> AddAsync(MatchHistoryRecord data);

    public void Update(MatchHistoryRecord data);

    public Task<bool> DeleteAsync(Guid id);

    public Task<int> SaveChangesAsync();
}