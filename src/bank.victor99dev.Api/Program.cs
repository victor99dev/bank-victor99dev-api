using System.Text.Json.Serialization;
using bank.victor99dev.Application.Interfaces.Repository;
using bank.victor99dev.Application.UseCases.Accounts.ActivateAccount;
using bank.victor99dev.Application.UseCases.Accounts.CreateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeactivateAccount;
using bank.victor99dev.Application.UseCases.Accounts.DeleteAccount;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountByCpf;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountById;
using bank.victor99dev.Application.UseCases.Accounts.GetAccountsPaged;
using bank.victor99dev.Application.UseCases.Accounts.RestoreAccount;
using bank.victor99dev.Application.UseCases.Accounts.UpdateAccount;
using bank.victor99dev.Infrastructure.Database.Context;
using bank.victor99dev.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<ICreateAccountUseCase, CreateAccountUseCase>();
builder.Services.AddScoped<IGetAccountByIdUseCase, GetAccountByIdUseCase>();
builder.Services.AddScoped<IGetAccountsPagedUseCase, GetAccountsPagedUseCase>();
builder.Services.AddScoped<IActivateAccountUseCase, ActivateAccountUseCase>();
builder.Services.AddScoped<IDeactivateAccountUseCase, DeactivateAccountUseCase>();
builder.Services.AddScoped<IDeleteAccountUseCase, DeleteAccountUseCase>();
builder.Services.AddScoped<IRestoreAccountUseCase, RestoreAccountUseCase>();
builder.Services.AddScoped<IGetAccountByCpfUseCase, GetAccountByCpfUseCase>();
builder.Services.AddScoped<IUpdateAccountUseCase, UpdateAccountUseCase>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Bank Victor99Dev API",
        Description = "Bank Victor99Dev API",
        Contact = new OpenApiContact
        {
            Name = "Victor99Dev",
            Url = new Uri("https://victor99dev.website/")
        }
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();