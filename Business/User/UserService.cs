using AutoMapper;
using Domain.Entities;
using Repository.User;

namespace Business.User;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        this._repository = repository;
        _mapper = mapper;
    }
    public async Task<UserModel> GetByIdAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id) ?? throw new Exception();
        return _mapper.Map<UserModel>(user);
    }

    public async Task<IReadOnlyCollection<UserModel>> GetAllAsync(int offset, int limit)
    {
        return _mapper.Map<IReadOnlyCollection<UserModel>>(await _repository.GetAllAsync(offset, limit));
    }

    public async Task<Guid> AddAsync(UserModel userModel)
    {
        if (await _repository.GetByEmailAsync(userModel.Email) is null)
        {
            throw new Exception($"User with email {userModel.Email} already exists");
        }
        
        var entity = _mapper.Map<UserRecord>(userModel);

        var result = await _repository.AddAsync(entity);

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await GetByIdAsync(id);
        return await _repository.DeleteAsync(id);
    }

    public async Task<UserModel> UpdateAsync(UserModel userModel)
    {
        var entity = await _repository.GetByIdAsync(userModel.Id)
                     ?? throw new Exception("user entity not found");

        _mapper.Map(userModel, entity);


        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return userModel;
    }
}