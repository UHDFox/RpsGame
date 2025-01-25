using Business.Models;

namespace Business.Game;

public interface IGameService
{
    Task<Guid> CreateMatchAsync(Guid hostId, decimal betAmount);
    Task<IEnumerable<MatchHistoryModel>> GetAllMatchesAsync();
    Task<bool> OperateMoneyTransaction(Guid senderId, Guid receiverId, decimal amount);
}