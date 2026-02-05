using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Domain.ValueObjects;

namespace bank.victor99dev.Tests.Domain.ValueObjects;

public class CpfValueObjectTests
{
    [Fact(DisplayName = "Should create CPF when value is valid")]
    public void ShouldCreateCpfWhenValueIsValid()
    {
        var cpf = "123.456.789-09";

        var result = new CpfValueObject(cpf);

        Assert.NotNull(result);
        Assert.Equal("12345678909", result.Value);
    }

    [Fact(DisplayName = "Should throw exception when CPF is null")]
    public void ShouldThrowExceptionWhenCpfIsNull()
    {
        string cpf = null!;

        var exception = Assert.Throws<DomainException>(() => new CpfValueObject(cpf));

        Assert.Equal("CPF is required.", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when CPF is empty")]
    public void ShouldThrowExceptionWhenCpfIsEmpty()
    {
        var cpf = string.Empty;

        var exception = Assert.Throws<DomainException>(() => new CpfValueObject(cpf));

        Assert.Equal("CPF is required.", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when CPF does not have exactly 11 digits")]
    public void ShouldThrowExceptionWhenCpfDoesNotHaveExactly11Digits()
    {
        var cpf = "1234567890";

        var exception = Assert.Throws<DomainException>(() => new CpfValueObject(cpf));

        Assert.Equal("CPF must contain exactly 11 digits.", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when CPF has all digits equal")]
    public void ShouldThrowExceptionWhenCpfHasAllDigitsEqual()
    {
        var cpf = "111.111.111-11";

        var exception = Assert.Throws<DomainException>(() => new CpfValueObject(cpf));

        Assert.Equal("CPF is invalid.", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when CPF check digits are invalid")]
    public void ShouldThrowExceptionWhenCpfCheckDigitsAreInvalid()
    {
        var cpf = "123.456.789-00";

        var exception = Assert.Throws<DomainException>(() => new CpfValueObject(cpf));

        Assert.Equal("CPF is invalid.", exception.Message);
    }
}