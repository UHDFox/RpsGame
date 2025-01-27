namespace Business.Infrastructure.Exceptions;

public class InvalidBetException : Exception
{
    public InvalidBetException(string message) : base(message) { }
}