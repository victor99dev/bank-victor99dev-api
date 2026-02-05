using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Domain.ValueObjects;

namespace bank.victor99dev.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public NameValueObject AccountName { get; private set; }
        public CpfValueObject Cpf { get; private set; } = default!;
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected Account() { }

        public static Account Create(NameValueObject accountName, CpfValueObject cpf)
        {
            return new Account
            {
                Id = Guid.NewGuid(),
                AccountName = accountName,
                Cpf = cpf,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(NameValueObject accountName)
        {
            AccountName = accountName;
            SetUpdatedAt();
        }

        public void Activate()
        {
            if(IsDeleted)
                throw new DomainException("Cannot activate a deleted account. Restore it before activating.");
            
            if (IsActive) return;

            IsActive = true;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("Account is already deactivated.");

            IsActive = false;
            SetUpdatedAt();
        }

        public void MarkAsDeleted()
        {
            if(IsDeleted)
                throw new DomainException("The account is already marked as deleted.");
            IsDeleted = true;
            IsActive = false;
            SetUpdatedAt();
        }

        public void Restore()
        {
            if(IsDeleted is false)
                throw new DomainException("Cannot restore a account that is not deleted.");
            IsDeleted = false;
            IsActive = true;
            SetUpdatedAt();
        }

        public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
    }
}