namespace bank.victor99dev.Application.UseCases.Accounts.Shared;

public sealed record AccountResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}