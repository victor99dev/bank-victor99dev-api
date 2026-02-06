using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;

namespace bank.victor99dev.Tests.Application.Shared;

public static class AccountRequests
{
    public static CreateAccountRequest Valid(string? name = null, string? cpf = null, int? seed = null)
        => new CreateAccountRequest
        {
            Name = name ?? "Victor Account",
            Cpf = cpf ?? CpfGenerator.FromSeed(seed ?? 1)
        };
}
