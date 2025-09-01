namespace BrewLog.Api.Services.Exceptions;

public class ServiceException : Exception
{
    public ServiceException(string message) : base(message)
    {
    }

    public ServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class NotFoundException : ServiceException
{
    public NotFoundException(string entityName, int id)
        : base($"{entityName} with ID {id} was not found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}

public class BusinessValidationException : ServiceException
{
    public BusinessValidationException(string message) : base(message)
    {
    }

    public BusinessValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class ReferentialIntegrityException : ServiceException
{
    public ReferentialIntegrityException(string message) : base(message)
    {
    }

    public ReferentialIntegrityException(string message, Exception innerException) : base(message, innerException)
    {
    }
}