namespace bank.victor99dev.Application.UseCases.Accounts.ChangeAccountCpf;

public sealed record class ChangeAccountCpfRequest
{
    public required string Cpf { get; init; }
}