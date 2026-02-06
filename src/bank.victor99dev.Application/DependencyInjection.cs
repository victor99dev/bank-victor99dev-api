using bank.victor99dev.Application.UseCases.Accounts.ActivateAccount;
using bank.victor99dev.Application.UseCases.Accounts.ChangeAccountCpf;
using bank.victor99dev.Application.UseCases.Accounts.ChangeAccountName;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeactivateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeleteAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountByCpf;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountById;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountsPaged;
using bank.victor99dev.Application.UseCases.Accounts.RestoreAccount;
using bank.victor99dev.Application.UseCases.Accounts.UpdateAccount;
using Microsoft.Extensions.DependencyInjection;

namespace bank.victor99dev.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateAccountUseCase, CreateAccountUseCase>();
        services.AddScoped<IGetAccountByIdUseCase, GetAccountByIdUseCase>();
        services.AddScoped<IGetAccountsPagedUseCase, GetAccountsPagedUseCase>();
        services.AddScoped<IActivateAccountUseCase, ActivateAccountUseCase>();
        services.AddScoped<IDeactivateAccountUseCase, DeactivateAccountUseCase>();
        services.AddScoped<IDeleteAccountUseCase, DeleteAccountUseCase>();
        services.AddScoped<IRestoreAccountUseCase, RestoreAccountUseCase>();
        services.AddScoped<IGetAccountByCpfUseCase, GetAccountByCpfUseCase>();
        services.AddScoped<IUpdateAccountUseCase, UpdateAccountUseCase>();
        services.AddScoped<IChangeAccountNameUseCase, ChangeAccountNameUseCase>();
        services.AddScoped<IChangeAccountCpfUseCase, ChangeAccountCpfUseCase>();

        return services;
    }
}