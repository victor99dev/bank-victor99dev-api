namespace bank.victor99dev.Application.UseCases.Accounts.CreateAccount;

public class CreateAccountRequest
{
    public required string Name {get; set; }
    public required string Cpf {get; set; }
}