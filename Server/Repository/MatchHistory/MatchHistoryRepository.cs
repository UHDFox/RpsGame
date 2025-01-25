using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository.MatchHistory;

public class MatchHistoryRepository : IMatchHistoryRepository
{
    private readonly GameServerDbContext _context;

    public MatchHistoryRepository(GameServerDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<MatchHistoryRecord>> GetAllAsync()
    {
        return await _context.MatchHistories.ToListAsync();
    }

    public async Task<IReadOnlyCollection<MatchHistoryRecord>> GetAllMatchesForUserAsync(string id)
    {
        return await _context.MatchHistories
            .Where(x => x.OpponentId.ToString() == id || x.HostId.ToString() == id)
            .ToListAsync();
    }

    public async Task<MatchHistoryRecord?> GetByIdAsync(Guid id)
    {
        return await _context.MatchHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Guid> AddAsync(MatchHistoryRecord data)
    {
        var result = await _context.MatchHistories.AddAsync(data);
        await SaveChangesAsync();
        return result.Entity.Id;
    }

    public void Update(MatchHistoryRecord data)
    {
        ;
        _context.MatchHistories.Update(data);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _context.MatchHistories.Remove((await GetByIdAsync(id))!);
        return await SaveChangesAsync() > 0;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}