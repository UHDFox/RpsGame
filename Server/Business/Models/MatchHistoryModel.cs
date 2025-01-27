namespace Business.Models;

public sealed class MatchHistoryModel
{
    public Guid Id { get; set; }
    
    public Guid HostId { get; set; }
    
    public Guid? OpponentId { get; set; }
    
    public string? Winner { get; set; }
    
    public decimal Bet { get; set; }
    
    public DateTimeOffset StartTime { get; set; }
    
    public MatchHistoryModel(Guid hostId, decimal bet, string? winner)
    {
        Id = Guid.NewGuid();
        HostId = hostId;
        Bet = bet;
        StartTime = DateTimeOffset.UtcNow;
        Winner = winner;
    }
}