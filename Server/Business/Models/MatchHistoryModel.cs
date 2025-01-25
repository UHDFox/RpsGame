namespace Business.Models;

public class MatchHistoryModel
{
    public Guid Id { get; set; }
    
    public Guid HostId { get; set; }
    
    public Guid? OpponentId { get; set; }
    
    public Guid? WinnerId { get; set; }
    
    public decimal Bet { get; set; }
    
    public DateTimeOffset StartTime { get; set; }
    
    public MatchHistoryModel(Guid hostId, decimal bet)
    {
        Id = Guid.NewGuid();
        HostId = hostId;
        Bet = bet;
        StartTime = DateTimeOffset.UtcNow;
    }
}