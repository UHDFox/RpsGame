using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository.GameTransactions;

public class GameTransactionsRepository : IGameTransactionsRepository
{
    private readonly GameServerDbContext _context;

    public GameTransactionsRepository(GameServerDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<GameTransactionsRecord>> GetAllAsync()
    {
        return await _context.GameTransactions.ToListAsync();
    }

    public async Task<GameTransactionsRecord?> GetByIdAsync(Guid id)
    {
        return await _context.GameTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Guid> AddAsync(GameTransactionsRecord data)
    {
        var result = await _context.GameTransactions.AddAsync(data);
        await SaveChangesAsync();
        return result.Entity.Id;
    }

    public void Update(GameTransactionsRecord data)
    {
        ;
        _context.GameTransactions.Update(data);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _context.GameTransactions.Remove((await GetByIdAsync(id))!);
        return await SaveChangesAsync() > 0;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}