namespace bank.victor99dev.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string error) : base(error)
    { }

    public static void When(bool hasErrors, string error)
    {
        if (hasErrors)
            throw new DomainException(error);
    }

}