namespace Domain.Entities;

public sealed class GameTransactionsRecord
{
    public Guid Id { get; set; }
    
    public Guid SenderId { get; set; }
    
    public Guid ReceiverId { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTimeOffset TransactionDate { get; set; } 
    
    public UserRecord? Sender { get; set; }
    
    public UserRecord? Receiver { get; set; }
    
    public GameTransactionsRecord(Guid senderId, Guid receiverId, decimal amount)
    {
        Id = Guid.NewGuid();
        SenderId = senderId;
        ReceiverId = receiverId;
        Amount = amount;
        TransactionDate = DateTimeOffset.UtcNow;
    }
}