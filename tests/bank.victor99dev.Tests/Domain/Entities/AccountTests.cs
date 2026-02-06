using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Domain.Exceptions;

namespace bank.victor99dev.Tests.Domain.Entities;

public class AccountTests
{
    [Fact(DisplayName = "Should create account when name and CPF are valid")]
    public void ShouldCreateAccountWhenNameAndCpfAreValid()
    {
        var account = Account.Create("Victor Hugo", "123.456.789-09");

        Assert.NotEqual(default, account.Id);
        Assert.Equal("Victor Hugo", account.AccountName.Value);
        Assert.Equal("12345678909", account.Cpf.Value);

        Assert.True(account.IsActive);
        Assert.False(account.IsDeleted);

        Assert.NotEqual(default, account.CreatedAt);
        Assert.Null(account.UpdatedAt);
    }

    [Fact(DisplayName = "Should update account details and set UpdatedAt when Update (PUT) is called")]
    public void ShouldUpdateAccountDetailsAndSetUpdatedAtWhenUpdatePutIsCalled()
    {
        var account = CreateAccount();
        Assert.Null(account.UpdatedAt);

        account.Update(
            accountName: "Updated Name",
            cpf: "987.654.321-00",
            isActive: false,
            isDeleted: false
        );

        Assert.Equal("Updated Name", account.AccountName.Value);
        Assert.Equal("98765432100", account.Cpf.Value);
        Assert.False(account.IsActive);
        Assert.False(account.IsDeleted);

        Assert.NotNull(account.UpdatedAt);
        Assert.True(account.UpdatedAt >= account.CreatedAt);
    }

    [Fact(DisplayName = "Should throw exception when calling Update (PUT) on a deleted account")]
    public void ShouldThrowExceptionWhenUpdatingPutOnADeletedAccount()
    {
        var account = CreateAccount();
        account.MarkAsDeleted();

        var ex = Assert.Throws<DomainException>(() =>
            account.Update(
                accountName: "Updated Name",
                cpf: "987.654.321-00",
                isActive: true,
                isDeleted: false
            )
        );

        Assert.Equal("Cannot update account details because the account is deleted. Restore it first.", ex.Message);
    }

    [Fact(DisplayName = "Should change account name and set UpdatedAt when ChangeName is called")]
    public void ShouldChangeAccountNameAndSetUpdatedAtWhenChangeNameIsCalled()
    {
        var account = CreateAccount();
        Assert.Null(account.UpdatedAt);

        account.ChangeName("New Name");

        Assert.Equal("New Name", account.AccountName.Value);
        Assert.NotNull(account.UpdatedAt);
        Assert.True(account.UpdatedAt >= account.CreatedAt);
    }

    [Fact(DisplayName = "Should throw exception when changing name of a deleted account")]
    public void ShouldThrowExceptionWhenChangingNameOfADeletedAccount()
    {
        var account = CreateAccount();
        account.MarkAsDeleted();

        var ex = Assert.Throws<DomainException>(() => account.ChangeName("Any Name"));

        Assert.Equal("Cannot change the account name because the account is deleted. Restore it first.", ex.Message);
    }

    [Fact(DisplayName = "Should change account CPF and set UpdatedAt when ChangeCpf is called")]
    public void ShouldChangeAccountCpfAndSetUpdatedAtWhenChangeCpfIsCalled()
    {
        var account = CreateAccount();
        Assert.Null(account.UpdatedAt);

        account.ChangeCpf("987.654.321-00");

        Assert.Equal("98765432100", account.Cpf.Value);
        Assert.NotNull(account.UpdatedAt);
        Assert.True(account.UpdatedAt >= account.CreatedAt);
    }

    [Fact(DisplayName = "Should throw exception when changing CPF of a deleted account")]
    public void ShouldThrowExceptionWhenChangingCpfOfADeletedAccount()
    {
        var account = CreateAccount();
        account.MarkAsDeleted();

        var ex = Assert.Throws<DomainException>(() => account.ChangeCpf("987.654.321-00"));

        Assert.Equal("Cannot change the account CPF because the account is deleted. Restore it first.", ex.Message);
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

        Assert.Equal("Account is already inactive.", exception.Message);
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

        Assert.Equal("Account is already deleted.", exception.Message);
    }

    [Fact(DisplayName = "Should throw exception when activating a deleted account")]
    public void ShouldThrowExceptionWhenActivatingADeletedAccount()
    {
        var account = CreateAccount();
        account.MarkAsDeleted();

        var exception = Assert.Throws<DomainException>(() => account.Activate());

        Assert.Equal("Cannot activate a deleted account. Restore it first.", exception.Message);
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

        Assert.Equal("Cannot restore an account that is not deleted.", exception.Message);
    }

    private static Account CreateAccount() =>
        Account.Create("Victor Hugo", "123.456.789-09");
}