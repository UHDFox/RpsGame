namespace Business.Models;

public sealed class MatchStatusInfo
{
    public string MatchId { get; set; } = "";
    public decimal Bet { get; set; }
    public bool IsWaitingForPlayer { get; set; }
}