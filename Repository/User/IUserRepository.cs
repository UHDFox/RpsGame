using Domain.Entities;

namespace Repository.User;

public interface IUserRepository
{
    public Task<IReadOnlyCollection<UserRecord>> GetAllAsync(int offset, int limit);

    public Task<UserRecord?> GetByEmailAsync(string email);

    public Task<UserRecord?> GetByIdAsync(Guid id);

    public Task<Guid> AddAsync(UserRecord data);

    public void Update(UserRecord data);

    public Task<bool> DeleteAsync(Guid id);

    public Task<int> SaveChangesAsync();
}