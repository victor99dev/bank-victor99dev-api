using bank.victor99dev.Domain.Entities;
using bank.victor99dev.Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;

namespace bank.victor99dev.Infrastructure.Database.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}