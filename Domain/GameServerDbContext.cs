using Domain.Entities;
using Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Domain;

public class GameServerDbContext : DbContext
{
    public GameServerDbContext(DbContextOptions<GameServerDbContext> options) : base(options)
    {
    }
    
    public DbSet<UserRecord> Users { get; set; }
    
    public DbSet<MatchHistoryRecord> MatchHistories { get; set; }
    
    public DbSet<GameTransactionsRecord> GameTransactions  { get; set; }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}