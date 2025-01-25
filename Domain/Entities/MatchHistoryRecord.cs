namespace Domain.Entities;

public sealed class MatchHistoryRecord
{
    public Guid Id { get; set; }
    
    public Guid HostId { get; set; }
    
    public Guid? OpponentId { get; set; }
    
    public Guid? WinnerId { get; set; }
    
    public decimal Bet { get; set; }
    
    public DateTimeOffset StartTime { get; set; }
    
    public UserRecord? Host { get; set; }
    
    public UserRecord? Opponent { get; set; }
    
    public MatchHistoryRecord(Guid hostId, decimal bet)
    {
        HostId = hostId;
        Bet = bet;
        StartTime = DateTimeOffset.UtcNow;
    }
}