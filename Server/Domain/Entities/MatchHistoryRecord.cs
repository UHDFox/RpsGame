using Domain.Infrastructure.Enums;

namespace Domain.Entities;

public sealed class MatchHistoryRecord
{
    public Guid Id { get; set; }
    
    public Guid HostId { get; set; }
    
    public Guid? OpponentId { get; set; }
    
    public string? Winner { get; set; }
    
    public decimal Bet { get; set; }
    
    public MatchStatus Status { get; set; }
    
    public DateTimeOffset StartTime { get; set; }
    
    public UserRecord? Host { get; set; }
    
    public UserRecord? Opponent { get; set; }
    
    public ICollection<string> PlayerMoves { get; set; } = new List<string>();
    
    public MatchHistoryRecord(Guid hostId, decimal bet, string? winner)
    {
        HostId = hostId;
        Bet = bet;
        StartTime = DateTimeOffset.UtcNow;
        Winner = winner;
    }
}