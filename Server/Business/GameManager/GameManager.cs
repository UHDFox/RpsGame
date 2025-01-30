using Business.Infrastructure.Exceptions;
using Business.Models;
using Domain.Entities;
using Domain.Infrastructure.Enums;
using Game;
using Repository.GameTransactions;
using Repository.MatchHistory;
using Repository.User;


namespace Business.GameManager;

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
        var host = await _userRepository.GetByIdAsync(hostId) 
                   ?? throw new UserNotFoundException("Host user not found.");

        if (betAmount <= 0)
            throw new InvalidBetException("Bet amount must be greater than zero.");

        var match = new MatchHistoryRecord(hostId, betAmount, null)
        {
            HostId = hostId,
            Bet = betAmount,
            PlayerMoves = new List<string>(),
            Status = MatchStatus.Postponed,
            StartTime = DateTime.UtcNow
        };

        await _matchHistoryRepository.AddAsync(match);
        await _matchHistoryRepository.SaveChangesAsync();
        
        await ProcessPlayerMoveAsync(match.Id.ToString(), hostMove, hostId.ToString() );
        
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
    try
    {
        var match = await _matchHistoryRepository.GetByIdAsync(Guid.Parse(matchId))
                    ?? throw new Exception("Match not found.");
        
        if (match.Status == MatchStatus.Finished)
        {
            throw new Exception("The match has already finished.");
        }
        
        if (match.Status == MatchStatus.Postponed)
        {
            var opponent = await _userRepository.GetByIdAsync(Guid.Parse(opponentId))
                           ?? throw new Exception($"Opponent with id {opponentId} not found.");
            
            if (match.OpponentId != Guid.Empty)
            {
                if(opponentId != match.HostId.ToString())
                {
                    match.OpponentId = Guid.Parse(opponentId);
                    match.Status = MatchStatus.GameStarted;  // Game started
                }
                
                
                _matchHistoryRepository.Update(match);
                await _userRepository.SaveChangesAsync();
                
                return new JoinMatchResponse()
                {
                    MatchId = match.Id.ToString(),
                    Status = MatchStatus.GameStarted.ToString(),
                };
            }
            else
            {
                return new JoinMatchResponse()
                {
                    MatchId = match.Id.ToString(),
                    Status = MatchStatus.Postponed.ToString(),
                };
            }
        }
        
        if (match.Status == MatchStatus.GameStarted)
        {
            var opponent = await _userRepository.GetByIdAsync(Guid.Parse(opponentId))
                           ?? throw new Exception("Opponent not found.");
            
            if (match.HostId == Guid.Parse(opponentId) || match.OpponentId == Guid.Parse(opponentId))
            {
                return new JoinMatchResponse()
                {
                    MatchId = match.Id.ToString(),
                    Status = MatchStatus.GameStarted.ToString(),
                };
            }
            else
            {
                throw new Exception("Player is not part of this match.");
            }
        }
        
        return new JoinMatchResponse()
        {
            MatchId = matchId, 
            Status = "Error: Unexpected error occurred" 
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error while joining the match: {ex.Message}");
        
        return new JoinMatchResponse()
        {
            MatchId = matchId,
            Status = $"Error: {ex.Message}"
        };
    }
}

    public async Task<int> CreateMatch(MatchHistoryModel request)
    {
        var record = new MatchHistoryRecord(request.HostId, request.Bet, null);
        
        await _matchHistoryRepository.AddAsync(record);
        
        return await _matchHistoryRepository.SaveChangesAsync(); 
    }


    public async Task<JoinMatchResponse> ProcessPlayerMoveAsync(string matchId, string playerMove, string playerId)
    {
        var match = await _matchHistoryRepository.GetByIdAsync(Guid.Parse(matchId))
                    ?? throw new Exception("Match not found.");

        if (!IsValidMove(playerMove))
            throw new InvalidMoveException($"Invalid player move: {playerMove}");
        
        if (match.HostId == Guid.Parse(playerId))
        {
            match.PlayerMoves.Add(playerMove); // Ход хоста
        }
        else if (match.OpponentId == Guid.Parse(playerId))
        {
            match.PlayerMoves.Add(playerMove); // Ход оппонента
        }
        else
        {
            throw new Exception("Player is not part of this match.");
        }

        // Если оба хода сделаны, определяем победителя
        if (match.PlayerMoves.Count == 2)
        {
            var winner = DetermineWinner(match.PlayerMoves.ElementAt(0), match.PlayerMoves.ElementAt(1));
            match.Winner = winner == "Player 1" ? match.HostId.ToString() : match.OpponentId.ToString();
            match.Status = MatchStatus.Finished;
            
            await ProcessBetAsync(match);
        }
        _matchHistoryRepository.Update(match);

        await _matchHistoryRepository.SaveChangesAsync();
        
        return new JoinMatchResponse()
        {
            MatchId = matchId,
            Status = match.Status.ToString(),
            Winner = match.Winner ?? "Tie"
        };
    }
    
    public async Task<IEnumerable<Game.MatchStatusInfo>> GetMatchesWithBetAndWaitingPlayerAsync()
    {
        var matches = await _matchHistoryRepository.GetAllAsync();
        
        // Создадим список с информацией о матчах
        var matchStatusList = matches.Select(match => new Game.MatchStatusInfo
        {
            MatchId = match.Id.ToString(),
            Bet = (double)match.Bet,
            IsWaitingForPlayer = match.OpponentId == Guid.Empty // Если OpponentId пуст, значит матч ожидает игрока
        });
    
        return matchStatusList;
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

     private bool IsValidMove(string move) => new[] { "К", "Н", "Б" }.Contains(move);

     private string DetermineWinner(string move1, string move2)
     {
         if (move1 == move2)
             return "Draw";

         if ((move1 == "К" && move2 == "Н") || (move1 == "Н" && move2 == "Б") || (move1 == "Б" && move2 == "К"))
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

         var loserId = match.HostId.ToString() == match.Winner ? match.OpponentId : match.HostId;
         
         var loser = await _userRepository.GetByIdAsync(Guid.Parse(loserId.ToString() 
                                            ?? throw new UserNotFoundException($"Couldn't find the player by id while determining winner")));
     
         decimal betAmount = match.Bet;
     
         winner.Balance += betAmount;
         loser.Balance -= betAmount;
     
         await _userRepository.SaveChangesAsync();  
     }
    
}
