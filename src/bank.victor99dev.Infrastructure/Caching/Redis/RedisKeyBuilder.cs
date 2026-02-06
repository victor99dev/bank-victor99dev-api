namespace bank.victor99dev.Infrastructure.Caching.Redis;

public static class RedisKeyBuilder
{
    public static string AccountById(Guid id) => $"accounts:id:{id}";

    public static string AccountByCpf(string cpf) => $"accounts:cpf:{cpf.Trim()}";
}