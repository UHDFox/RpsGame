using Business.Models;
using Domain.Entities;
using Game;

namespace Business.Game;

public interface IGameManager
{
    Task<Guid> CreateMatchAsync(Guid hostId, decimal betAmount, string hostMove);
    Task<IEnumerable<MatchHistoryModel>> GetAllMatchesAsync();
    Task<IReadOnlyCollection<MatchHistory>> GetAllMatchesForUserAsync(string id);

    Task<bool> TransferMoney(Guid senderId, Guid receiverId, decimal amount);

    public Task<decimal> GetBalanceAsync(string userId);
    
    Task<JoinMatchResponse> ProcessPlayerMoveAsync(string matchId, string playerMove, string opponentId);
    
    Task<JoinMatchResponse> JoinMatchAsync(string matchId, string opponentId);
}