namespace Business.Infrastructure.Exceptions;

public class GameNotFoundException : Exception
{
    public GameNotFoundException(string message) : base(message) { }
}