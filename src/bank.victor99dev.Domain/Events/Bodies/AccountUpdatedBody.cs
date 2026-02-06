namespace bank.victor99dev.Domain.Events.Bodies;

public sealed record class AccountUpdatedBody
{
    public Guid AccountId { get; init; }
    public string Cpf { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}