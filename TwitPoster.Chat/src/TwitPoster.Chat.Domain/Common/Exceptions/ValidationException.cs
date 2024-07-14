namespace TwitPoster.Chat.Domain.Common.Exceptions;

public class TwitPosterValidationException : Exception
{
    public TwitPosterValidationException(string message) : base(message)
    {
        
    }
    
    public TwitPosterValidationException(string message, Exception innerException) : base(message, innerException)
    {
        
    }
}

