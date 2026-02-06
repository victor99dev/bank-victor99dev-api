using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Domain.ValueObjects;

namespace bank.victor99dev.Tests.Domain.ValueObjects;

public class NameValueObjectTests
{
    [Fact(DisplayName = "Should create NameValueObject when value is valid and trimmed")]
    public void ShouldCreateNameWhenValueIsValidAndTrimmed()
    {
        var name = "  Victor  ";

        var result = new NameValueObject(name);

        Assert.NotNull(result);
        Assert.Equal("Victor", result.Value);
    }

    [Fact(DisplayName = "Should throw exception when name is null")]
    public void ShouldThrowExceptionWhenNameIsNull()
    {
        string name = null!;

        var exception = Assert.Throws<DomainException>(() => new NameValueObject(name));

        Assert.Equal("Name is requered", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when name is empty")]
    public void ShouldThrowExceptionWhenNameIsEmpty()
    {
        var name = string.Empty;

        var exception = Assert.Throws<DomainException>(() => new NameValueObject(name));

        Assert.Equal("Name is requered", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when name has less than 3 characters")]
    public void ShouldThrowExceptionWhenNameHasLessThan3Characters()
    {
        var name = "ab";

        var exception = Assert.Throws<DomainException>(() => new NameValueObject(name));

        Assert.Equal("Name must contain at least 3 characters.", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when name has more than 20 characters")]
    public void ShouldThrowExceptionWhenNameHasMoreThan20Characters()
    {
        var name = new string('a', 101);

        var exception = Assert.Throws<DomainException>(() => new NameValueObject(name));

        Assert.Equal("Name must contain at most 100 characters.", exception.Message);
    }
}