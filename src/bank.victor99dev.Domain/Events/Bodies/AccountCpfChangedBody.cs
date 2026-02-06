namespace bank.victor99dev.Domain.Events.Bodies;

public sealed record class AccountCpfChangedBody
{
    public Guid AccountId { get; init; }
    public string OldCpf { get; init; } = string.Empty;
    public string NewCpf { get; init; } = string.Empty;
}