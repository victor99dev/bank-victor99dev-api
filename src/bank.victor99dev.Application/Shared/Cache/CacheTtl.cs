namespace bank.victor99dev.Application.Shared.Cache;

public static class CacheTtl
{
    public static TimeSpan UntilEndOfDayUtc()
    {
        var now = DateTimeOffset.UtcNow;
        var endOfDay = new DateTimeOffset(now.Year, now.Month, now.Day, 23, 59, 59, TimeSpan.Zero);
        var ttl = endOfDay - now;
        return ttl <= TimeSpan.Zero ? TimeSpan.FromMinutes(1) : ttl;
    }
}
