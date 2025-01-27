using Business.Models;

namespace Business.User;

public interface IUserService
{
    Task<TransferMoneyModel> TransferMoneyAsync(TransferMoneyModel request);

    Task<IReadOnlyCollection<UserModel>> GetAllAsync(int offset, int limit);

    Task<UserModel> GetByIdAsync(Guid id);

    Task<Guid> AddAsync(UserModel model);

    Task<UserModel> UpdateAsync(UserModel model);

    Task<bool> DeleteAsync(Guid id);
}