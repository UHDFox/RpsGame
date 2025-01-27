namespace Business.Models;

public sealed class TransferMoneyModel
{
    public string SenderId { get; set; } = "";
    public string ReceiverId { get; set; } = "";
    public double Amount { get; set; }
}