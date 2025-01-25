using Business.Models;
using Domain.Entities;
using Repository.GameTransactions;
using Repository.MatchHistory;
using Repository.User;

namespace Business.Game;

public sealed class GameService : IGameService
{
    private readonly IMatchHistoryRepository _matchHistoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGameTransactionsRepository _gameTransactionsRepository;

    public GameService(
        IMatchHistoryRepository matchHistoryRepository,
        IUserRepository userRepository,
        IGameTransactionsRepository gameTransactionsRepository)
    {
        _matchHistoryRepository = matchHistoryRepository;
        _userRepository = userRepository;
        _gameTransactionsRepository = gameTransactionsRepository;
    }

    public async Task<Guid> CreateMatchAsync(Guid hostId, decimal betAmount)
    {
        var host = await _userRepository.GetByIdAsync(hostId);
        if (host == null)
        {
            throw new Exception("Host user not found.");
        }

        var match = new MatchHistoryRecord(host.Id, betAmount);

        await _matchHistoryRepository.AddAsync(match);
        return match.Id;
    }

    public async Task<IEnumerable<MatchHistoryModel>> GetAllMatchesAsync()
    {
        var matches = await _matchHistoryRepository.GetAllAsync();
        
        return matches.Select(m => new MatchHistoryModel(m.HostId, m.Bet)
        {
            Id = m.Id,
            HostId = m.HostId,
            Bet = m.Bet,
            StartTime = m.StartTime,
            WinnerId = m.WinnerId
        });
    }

    public async Task<bool> OperateMoneyTransaction(Guid senderId, Guid receiverId, decimal amount)
    {
        var sender = await _userRepository.GetByIdAsync(senderId);
        var receiver = await _userRepository.GetByIdAsync(receiverId);

        if (sender == null || receiver == null)
        {
            throw new Exception("One or both users not found.");
        }

        if (sender.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        
        sender.Balance -= amount;

        
        receiver.Balance += amount;

        
        var transaction = new GameTransactionsRecord(senderId, receiverId, amount)
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            Amount = amount,
            TransactionDate = DateTimeOffset.UtcNow
        };

        await _gameTransactionsRepository.AddAsync(transaction);
        await _userRepository.SaveChangesAsync();
        
        return true;
    }
}
