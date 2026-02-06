namespace bank.victor99dev.Application.UseCases.Accounts.ChangeAccountName;

public sealed record class ChangeAccountNameRequest
{
    public required string Name { get; init; }
}