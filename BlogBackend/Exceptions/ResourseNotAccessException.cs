namespace BlogBackend.Exceptions;

public class ResourceNotAccessException: Exception
{
    public ResourceNotAccessException()
    {
    }

    public ResourceNotAccessException(string message) : base(message)
    {
    }

    public ResourceNotAccessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}