using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Domain.Interfaces.Events;

namespace bank.victor99dev.Application.Interfaces.Messaging;

public interface  IAccountEventFactory
{
    IDomainEvent Created(Account account);
    IDomainEvent Activated(Account account);
    IDomainEvent Deactivated(Account account);
    IDomainEvent Deleted(Account account);
    IDomainEvent Restored(Account account);
    IDomainEvent CpfChanged(Account account, string oldCpf);
    IDomainEvent NameChanged(Account account, string oldName);
    IDomainEvent Updated(Account account);
}