using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EntitiesConfiguration;

public class GameTransactionsConfiguration : IEntityTypeConfiguration<GameTransactionsRecord>
{
    public void Configure(EntityTypeBuilder<GameTransactionsRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x => x.SenderId);

        builder.HasOne(x => x.Receiver)
            .WithMany()
            .HasForeignKey(x => x.ReceiverId);
    }
}