using bank.victor99dev.Domain.Exceptions;

namespace bank.victor99dev.Domain.ValueObjects;

public class NameValueObject
{
    public string Value {get;}

    public NameValueObject(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            throw new DomainException("Name is requered");
        
        value = value.Trim();

        if (value.Length < 3)
            throw new DomainException($"Name must contain at least 3 characters.");

        if (value.Length > 20)
            throw new DomainException($"Name must contain at most 20 characters.");

        Value = value;
    }
}