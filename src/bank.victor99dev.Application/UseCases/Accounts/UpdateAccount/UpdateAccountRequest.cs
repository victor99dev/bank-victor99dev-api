namespace bank.victor99dev.Application.UseCases.Accounts.UpdateAccount;

public sealed record UpdateAccountRequest
{
    public required string Name { get; init; }
    public required string Cpf { get; init; }
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
}