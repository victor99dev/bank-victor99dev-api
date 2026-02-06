namespace bank.victor99dev.Application.Shared.Cache;

public static class CacheTtl
{
    private static readonly TimeSpan MinimumCacheDuration = TimeSpan.FromMinutes(1);

    public static TimeSpan CacheExpiresAtEndOfDayUtc()
    {
        var now = DateTimeOffset.UtcNow;

        var endOfDay = new DateTimeOffset(now.Year, now.Month, now.Day, 23, 59, 59, TimeSpan.Zero);

        var ttl = endOfDay - now;

        return ttl > TimeSpan.Zero ? ttl : MinimumCacheDuration;
    }
}
