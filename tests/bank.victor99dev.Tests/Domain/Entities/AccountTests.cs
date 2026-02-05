using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Domain.Exceptions;
using bank.victor99dev.Domain.ValueObjects;

namespace bank.victor99dev.Tests.Domain.Entities;

public class AccountTests
{
    [Fact(DisplayName = "Should create account when name and CPF are valid")]
    public void ShouldCreateAccountWhenNameAndCpfAreValid()
    {
        var name = CreateName("Victor Hugo");
        var cpf = CreateCpf("123.456.789-09");

        var account = Account.Create(name, cpf);

        Assert.NotEqual(default, account.Id);
        Assert.Equal("Victor Hugo", account.AccountName.Value);
        Assert.Equal("12345678909", account.Cpf.Value);

        Assert.True(account.IsActive);
        Assert.False(account.IsDeleted);

        Assert.NotEqual(default, account.CreatedAt);
        Assert.Null(account.UpdatedAt);
    }

    [Fact(DisplayName = "Should update account name and set UpdatedAt when Update is called")]
    public void ShouldUpdateAccountNameAndSetUpdatedAtWhenUpdateIsCalled()
    {
        var account = CreateAccount();
        Assert.Null(account.UpdatedAt);

        account.Update(CreateName("New Name"));

        Assert.Equal("New Name", account.AccountName.Value);
        Assert.NotNull(account.UpdatedAt);
        Assert.True(account.UpdatedAt >= account.CreatedAt);
    }

    [Fact(DisplayName = "Should deactivate account and set UpdatedAt when Deactivate is called")]
    public void ShouldDeactivateAccountAndSetUpdatedAtWhenDeactivateIsCalled()
    {
        var account = CreateAccount();
        Assert.Null(account.UpdatedAt);

        account.Deactivate();

        Assert.False(account.IsActive);
        Assert.NotNull(account.UpdatedAt);
        Assert.True(account.UpdatedAt >= account.CreatedAt);
    }

    [Fact(DisplayName = "Should throw exception when deactivating an already deactivated account")]
    public void ShouldThrowExceptionWhenDeactivatingAnAlreadyDeactivatedAccount()
    {
        var account = CreateAccount();
        account.Deactivate();

        var exception = Assert.Throws<DomainException>(() => account.Deactivate());

        Assert.Equal("Account is already deactivated.", exception.Message);
    }

    [Fact(DisplayName = "Should activate a deactivated account and set UpdatedAt when Activate is called")]
    public void ShouldActivateDeactivatedAccountAndSetUpdatedAtWhenActivateIsCalled()
    {
        var account = CreateAccount();
        account.Deactivate();

        var previousUpdatedAt = account.UpdatedAt;
        Assert.NotNull(previousUpdatedAt);

        account.Activate();

        Assert.True(account.IsActive);
        Assert.NotNull(account.UpdatedAt);
        Assert.True(account.UpdatedAt >= previousUpdatedAt);
        Assert.True(account.UpdatedAt >= account.CreatedAt);
    }

    [Fact(DisplayName = "Should do nothing when activating an already active account")]
    public void ShouldDoNothingWhenActivatingAnAlreadyActiveAccount()
    {
        var account = CreateAccount();
        var previousUpdatedAt = account.UpdatedAt;

        account.Activate();

        Assert.True(account.IsActive);
        Assert.Equal(previousUpdatedAt, account.UpdatedAt);
    }

    [Fact(DisplayName = "Should mark account as deleted, deactivate it, and set UpdatedAt when MarkAsDeleted is called")]
    public void ShouldMarkAccountAsDeletedDeactivateItAndSetUpdatedAtWhenMarkAsDeletedIsCalled()
    {
        var account = CreateAccount();
        Assert.Null(account.UpdatedAt);

        account.MarkAsDeleted();

        Assert.True(account.IsDeleted);
        Assert.False(account.IsActive);
        Assert.NotNull(account.UpdatedAt);
        Assert.True(account.UpdatedAt >= account.CreatedAt);
    }

    [Fact(DisplayName = "Should throw exception when marking an already deleted account as deleted")]
    public void ShouldThrowExceptionWhenMarkingAlreadyDeletedAccountAsDeleted()
    {
        var account = CreateAccount();
        account.MarkAsDeleted();

        var exception = Assert.Throws<DomainException>(() => account.MarkAsDeleted());

        Assert.Equal("The account is already marked as deleted.", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when activating a deleted account")]
    public void ShouldThrowExceptionWhenActivatingADeletedAccount()
    {
        var account = CreateAccount();
        account.MarkAsDeleted();

        var exception = Assert.Throws<DomainException>(() => account.Activate());

        Assert.Equal("Cannot activate a deleted account. Restore it before activating.", exception.Message);
    }

    [Fact(DisplayName = "Should restore a deleted account, activate it, and set UpdatedAt when Restore is called")]
    public void ShouldRestoreDeletedAccountActivateItAndSetUpdatedAtWhenRestoreIsCalled()
    {
        var account = CreateAccount();
        account.MarkAsDeleted();

        var previousUpdatedAt = account.UpdatedAt;
        Assert.NotNull(previousUpdatedAt);

        account.Restore();

        Assert.False(account.IsDeleted);
        Assert.True(account.IsActive);
        Assert.NotNull(account.UpdatedAt);

        Assert.True(account.UpdatedAt >= previousUpdatedAt);
        Assert.True(account.UpdatedAt >= account.CreatedAt);
    }

    [Fact(DisplayName = "Should throw exception when restoring an account that is not deleted")]
    public void ShouldThrowExceptionWhenRestoringAccountThatIsNotDeleted()
    {
        var account = CreateAccount();

        var exception = Assert.Throws<DomainException>(() => account.Restore());

        Assert.Equal("Cannot restore a account that is not deleted.", exception.Message);
    }

    private static Account CreateAccount()
    {
        return Account.Create(
            accountName: CreateName("Victor Hugo"),
            cpf: CreateCpf("123.456.789-09"));
    }

    private static NameValueObject CreateName(string value) => new(value);

    private static CpfValueObject CreateCpf(string value) => new(value);
}
