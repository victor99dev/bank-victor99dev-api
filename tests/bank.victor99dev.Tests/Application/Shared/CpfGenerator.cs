namespace bank.victor99dev.Tests.Application.Shared;

/// <summary>
/// Helper responsible for generating valid Brazilian CPF numbers for tests.
/// The generated values respect the official validation rules, including check digits.
/// </summary>
public static class CpfGenerator
{
    /// <summary>
    /// Generates a valid 11-digit CPF based on a 9-digit numeric base.
    /// The last two digits are calculated as check digits according to the official CPF algorithm.
    /// </summary>
    /// <param name="base9">
    /// A string containing exactly 9 numeric digits that will be used as the base of the CPF.
    /// Non-numeric characters are ignored.
    /// </param>
    /// <returns>A valid 11-digit CPF string.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the base is null, empty, or does not contain exactly 9 digits.
    /// </exception>
    public static string FromBase9(string base9)
    {
        if (string.IsNullOrWhiteSpace(base9))
            throw new ArgumentException("base9 is required.", nameof(base9));

        var digits = OnlyDigits(base9);

        if (digits.Length != 9)
            throw new ArgumentException("base9 must have exactly 9 digits.", nameof(base9));

        // Avoid the "all same digits" case (e.g., 000000000)
        if (AllSameDigits(digits))
            digits = "123456789";

        var d1 = CalculateDigit(digits, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 });
        var d2 = CalculateDigit(digits + d1, new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 });

        return $"{digits}{d1}{d2}";
    }

    /// <summary>
    /// Generates a valid CPF from an integer seed.
    /// Useful for creating multiple distinct CPFs in loops and parameterized tests.
    /// </summary>
    /// <param name="seed">
    /// Numeric seed used to derive the 9-digit base of the CPF.
    /// </param>
    /// <returns>A valid 11-digit CPF string.</returns>
    public static string FromSeed(int seed)
    {
        var base9 = (seed % 1_000_000_000).ToString("D9");
        return FromBase9(base9);
    }

    /// <summary>
    /// Removes all non-numeric characters from the input string.
    /// </summary>
    private static string OnlyDigits(string input)
    {
        var chars = input.Where(char.IsDigit).ToArray();
        return new string(chars);
    }

    /// <summary>
    /// Checks whether all digits in the string are equal (e.g., "111111111").
    /// Such values are considered invalid CPFs.
    /// </summary>
    private static bool AllSameDigits(string digits)
    {
        for (int i = 1; i < digits.Length; i++)
            if (digits[i] != digits[0])
                return false;
        return true;
    }

    /// <summary>
    /// Calculates a CPF check digit using the official weighting algorithm.
    /// </summary>
    /// <param name="input">Numeric string used in the calculation.</param>
    /// <param name="weights">Array of weights applied to each digit.</param>
    /// <returns>The calculated check digit.</returns>
    private static int CalculateDigit(string input, int[] weights)
    {
        int sum = 0;

        for (int i = 0; i < weights.Length; i++)
            sum += (input[i] - '0') * weights[i];

        var mod = sum % 11;
        return mod < 2 ? 0 : 11 - mod;
    }
}
