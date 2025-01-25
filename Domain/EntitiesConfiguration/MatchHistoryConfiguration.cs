using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EntitiesConfiguration;

public class MatchHistoryConfiguration : IEntityTypeConfiguration<MatchHistoryRecord>
{
    public void Configure(EntityTypeBuilder<MatchHistoryRecord> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Host)
            .WithMany()
            .HasForeignKey(m => m.HostId);
        
        builder.HasOne(m => m.Opponent)
            .WithMany()
            .HasForeignKey(m => m.OpponentId);
    }
}