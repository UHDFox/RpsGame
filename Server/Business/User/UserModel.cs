namespace Business.User;

public class UserModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public string Email { get; set; }
    public decimal Balance { get; set; }
    
    public UserModel(string name, string email, decimal balance)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Balance = balance;
    }
}