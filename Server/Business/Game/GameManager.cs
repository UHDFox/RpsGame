using Business.Infrastructure.Exceptions;
using Business.Models;
using Domain.Entities;
using Domain.Infrastructure.Enums;
using Game;
using Repository.GameTransactions;
using Repository.MatchHistory;
using Repository.User;

namespace Business.Game;

public sealed class GameManager : IGameManager
{
    private readonly IMatchHistoryRepository _matchHistoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGameTransactionsRepository _gameTransactionsRepository;

    public GameManager(
        IMatchHistoryRepository matchHistoryRepository,
        IUserRepository userRepository,
        IGameTransactionsRepository gameTransactionsRepository)
    {
        _matchHistoryRepository = matchHistoryRepository;
        _userRepository = userRepository;
        _gameTransactionsRepository = gameTransactionsRepository;
    }

    public async Task<Guid> CreateMatchAsync(Guid hostId, decimal betAmount, string hostMove)
    {
        var host = await _userRepository.GetByIdAsync(hostId);
        if (host == null)
        {
            throw new Exception("Host user not found.");
        }

        // Validate the host's move
        if (!IsValidMove(hostMove))
        {
            throw new InvalidMoveException("Invalid move.");
        }

        // Create the match record and store the host's move
        var match = new MatchHistoryRecord(host.Id, betAmount, hostMove)
        {
            Status = MatchStatus.Postponed, 
            PlayerMoves = new[] { hostMove }
        };
        
        await _matchHistoryRepository.AddAsync(match);

        // Return the created match ID
        return match.Id;
    }

    public async Task<IEnumerable<MatchHistoryModel>> GetAllMatchesAsync()
    {
        var matches = await _matchHistoryRepository.GetAllAsync();
        
        return matches.Select(m => new MatchHistoryModel(m.HostId, m.Bet, null)
        {
            Id = m.Id,
            HostId = m.HostId,
            Bet = m.Bet,
            StartTime = m.StartTime,
            Winner = m.Winner
        });
    }
    
    public async Task<IReadOnlyCollection<MatchHistory>> GetAllMatchesForUserAsync(string id)
    {

        var user = _userRepository.GetByIdAsync(Guid.Parse(id))
            ?? throw new UserNotFoundException("User does not exist.");
        
        var matches = await _matchHistoryRepository.GetAllMatchesForUserAsync(id);
        
        return matches.Select(match => new MatchHistory()
        {
            MatchId = match.Id.ToString(),
            Bet = (double)match.Bet,
            Winner = match.Winner?.ToString() ?? "Tie",  // Ничья
            StartTime = match.StartTime.ToString()
        }).ToList();
    }
    

    public async Task<bool> TransferMoney(Guid senderId, Guid receiverId, decimal amount)
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
    
    public async Task<decimal> GetBalanceAsync(string userId)
    {
        return await _userRepository.GetBalanceAsync(userId);
    }

    
    public async Task<JoinMatchResponse> JoinMatchAsync(string matchId, string opponentId)
    {
        var match = await _matchHistoryRepository.GetByIdAsync(Guid.Parse(matchId))
                    ?? throw new Exception("Match not found.");

        // Проверка на статус матча
        if (match.Status != MatchStatus.Postponed)
            throw new Exception("This match is already ongoing.");

        var opponent = await _userRepository.GetByIdAsync(Guid.Parse(opponentId))
                       ?? throw new Exception("Opponent not found.");
        
        match.Opponent = opponent;
        match.Status = MatchStatus.Pending;  // Обновляем статус матча 

        _matchHistoryRepository.Update(match);
        await _userRepository.SaveChangesAsync();

        return new JoinMatchResponse
        {
            MatchId = matchId,
            Status = "Ongoing"
        };
    }
    
    public async Task<JoinMatchResponse> ProcessPlayerMoveAsync(string matchId, string playerMove, string opponentId)
    {
        var match = await _matchHistoryRepository.GetByIdAsync(Guid.Parse(matchId))
                    ?? throw new Exception("Match not found while processing player move");

        if (match == null)
            throw new Exception("Match not found");

        if (!IsValidMove(playerMove))
            throw new InvalidMoveException("Invalid move");

        if (match.PlayerMoves.Count == 0 || match.PlayerMoves.Count == 1)
        {
            match.PlayerMoves.Add(playerMove);
        }

        if (match.PlayerMoves.Count == 2)
        {
            var winner = DetermineWinner(match.PlayerMoves.First(), match.PlayerMoves.Last());
            if (winner == "Player 1")
            {
                winner = match.HostId.ToString();
            }
            else
            {
                //winner = 
            }
            match.Winner = winner;
            match.Status = MatchStatus.Finished;
            match.OpponentId = Guid.Parse(opponentId);

            _matchHistoryRepository.Update(match);
            await ProcessBetAsync(match);
        }

        return new JoinMatchResponse()
        {
            Winner = match.Winner,
            MatchId = matchId,
            Status = MatchStatus.Finished.ToString(),
        };
    }

     public async Task<IEnumerable<MatchHistory>> GetMatchHistoryAsync(string userId)
     {
         var matches = await _matchHistoryRepository.GetAllMatchesForUserAsync(userId);

         return matches.Select(match => new MatchHistory()
         {
             MatchId = match.Id.ToString(),
             Bet = (double)match.Bet,
             Winner = match.Winner?.ToString() ?? "Tie",  // Ничья
             StartTime = match.StartTime.ToString()
         });
     }

     private bool IsValidMove(string move) => new[] { "Камень", "Ножницы", "Бумага" }.Contains(move);

     private string DetermineWinner(string move1, string move2)
     {
         if (move1 == move2)
             return "Draw";

         if ((move1 == "Камень" && move2 == "Ножницы") || (move1 == "Ножницы" && move2 == "Бумага") || (move1 == "Бумага" && move2 == "Камень"))
             return "Player 1";

         return "Player 2";
     }
     
     private async Task ProcessBetAsync(MatchHistoryRecord match)
     {
         // Исходим из того, что победитель определен. Тогда проигравшим будет оставшийся игрок :)
         if (match.Winner == null)
         {
             throw new UserNotFoundException("Winner not determined");
         }

         var winner = await _userRepository.GetByIdAsync(Guid.Parse(match.Winner)) 
             ?? throw new UserNotFoundException($"Couldn't find user with email{match.Winner}");
         
         var loser = match.HostId == winner.Id ? winner : match.Host
             ?? throw new UserNotFoundException($"Couldn't find the player who lost");
         
         decimal betAmount = match.Bet;
         
         winner.Balance += betAmount;
         loser.Balance -= betAmount;
         
         await _userRepository.SaveChangesAsync();
     }
    
}
