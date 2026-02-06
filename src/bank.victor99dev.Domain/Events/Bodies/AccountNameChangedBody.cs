namespace bank.victor99dev.Domain.Events.Bodies;

public sealed record class AccountNameChangedBody
{
    public Guid AccountId { get; init; }
    public string OldName { get; init; } = string.Empty;
    public string NewName { get; init; } = string.Empty;
}