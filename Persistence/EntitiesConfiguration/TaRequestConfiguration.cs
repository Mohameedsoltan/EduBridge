using EduBridge.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduBridge.Persistence.EntitiesConfiguration;

public class TaRequestConfiguration : IEntityTypeConfiguration<TARequest>
{
    public void Configure(EntityTypeBuilder<TARequest> builder)
    {
        builder.HasOne(x => x.Team)
            .WithMany(x => x.TARequests)
            .HasForeignKey(x => x.TeamId);

        builder.HasOne(x => x.TA)
            .WithMany()
            .HasForeignKey(x => x.TAId);
    }
}