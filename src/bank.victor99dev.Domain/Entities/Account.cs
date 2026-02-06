using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Domain.ValueObjects;

namespace bank.victor99dev.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public NameValueObject AccountName { get; private set; }
    public CpfValueObject Cpf { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Account() { }

    public static Account Create(string accountName, string cpf)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            AccountName = new NameValueObject(accountName),
            Cpf = new CpfValueObject(cpf),
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string accountName, string cpf, bool isActive, bool isDeleted)
    {
        if (IsDeleted)
            throw new DomainException("Cannot update account details because the account is deleted. Restore it first.");

        AccountName = new NameValueObject(accountName);
        Cpf = new CpfValueObject(cpf);
        IsActive = isActive;
        IsDeleted = isDeleted;

        SetUpdatedAt();
    }

    public void ChangeName(string accountName)
    {
        if (IsDeleted)
            throw new DomainException("Cannot change the account name because the account is deleted. Restore it first.");

        AccountName = new NameValueObject(accountName);
        SetUpdatedAt();
    }

    public void ChangeCpf(string cpf)
    {
        if (IsDeleted)
            throw new DomainException("Cannot change the account CPF because the account is deleted. Restore it first.");

        Cpf = new CpfValueObject(cpf);
        SetUpdatedAt();
    }

    public void Activate()
    {
        if (IsDeleted)
            throw new DomainException("Cannot activate a deleted account. Restore it first.");

        if (IsActive) return;

        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Account is already inactive.");

        IsActive = false;
        SetUpdatedAt();
    }

    public void MarkAsDeleted()
    {
        if (IsDeleted)
            throw new DomainException("Account is already deleted.");
        IsDeleted = true;
        IsActive = false;
        SetUpdatedAt();
    }

    public void Restore()
    {
        if (IsDeleted is false)
            throw new DomainException("Cannot restore an account that is not deleted.");
        IsDeleted = false;
        IsActive = true;
        SetUpdatedAt();
    }

    public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}