using System.Text.RegularExpressions;
using bank.victor99dev.Domain.Exceptions;

namespace bank.victor99dev.Domain.ValueObjects;

public sealed class CpfValueObject
{
    public string Value { get; }

    public CpfValueObject(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("CPF is required.");

        var digits = OnlyDigits(value);

        if (digits.Length != 11)
            throw new DomainException("CPF must contain exactly 11 digits.");

        if (AllSameDigits(digits))
            throw new DomainException("CPF is invalid.");

        if (!HasValidCheckDigits(digits))
            throw new DomainException("CPF is invalid.");

        Value = digits;
    }

    private static string OnlyDigits(string input) =>
        Regex.Replace(input, @"\D", "");

    private static bool AllSameDigits(string digits)
    {
        for (int i = 1; i < digits.Length; i++)
            if (digits[i] != digits[0])
                return false;
        return true;
    }

    private static bool HasValidCheckDigits(string digits)
    {
        var d1 = CalculateDigit(digits.Substring(0, 9), [10, 9, 8, 7, 6, 5, 4, 3, 2]);

        var d2 = CalculateDigit(digits.Substring(0, 9) + d1, [11, 10, 9, 8, 7, 6, 5, 4, 3, 2]);

        return digits[9] == (char)('0' + d1) && digits[10] == (char)('0' + d2);
    }

    private static int CalculateDigit(string input, int[] weights)
    {
        int sum = 0;

        for (int i = 0; i < weights.Length; i++)
            sum += (input[i] - '0') * weights[i];

        var mod = sum % 11;
        return mod < 2 ? 0 : 11 - mod;
    }
}
