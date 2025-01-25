namespace Business.User;

public interface IUserService
{
    Task<IReadOnlyCollection<UserModel>> GetAllAsync(int offset, int limit);

    Task<UserModel> GetByIdAsync(Guid id);

    Task<Guid> AddAsync(UserModel model);

    Task<UserModel> UpdateAsync(UserModel model);

    Task<bool> DeleteAsync(Guid id);
}