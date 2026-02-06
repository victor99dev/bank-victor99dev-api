namespace bank.victor99dev.Application.UseCases.Accounts.CreateAccount;

public sealed record class CreateAccountRequest
{
    public required string Name { get; init; }
    public required string Cpf { get; init; }
}