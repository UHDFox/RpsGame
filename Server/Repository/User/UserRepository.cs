using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository.User;

public sealed class UserRepository : IUserRepository
{
    private readonly GameServerDbContext _context;

    public UserRepository(GameServerDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UserRecord>> GetAllAsync(int offset, int limit)
    {
        return await _context.Users.Skip(offset).Take(limit).ToListAsync();
    }

    public async Task<UserRecord?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<UserRecord?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<decimal> GetBalanceAsync(string userId)
    {
        return (await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == userId) 
                ?? throw new Exception("User not found")).Balance;
    }

    public async Task<Guid> AddAsync(UserRecord data)
    {
        var result = await _context.Users.AddAsync(data);
        await SaveChangesAsync();
        return result.Entity.Id;
    }

    public void Update(UserRecord data)
    {
        ;
        _context.Users.Update(data);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _context.Users.Remove((await GetByIdAsync(id))!);
        return await SaveChangesAsync() > 0;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
