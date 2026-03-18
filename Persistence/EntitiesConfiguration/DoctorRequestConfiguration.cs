using EduBridge.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduBridge.Persistence.EntitiesConfiguration;

public class DoctorRequestConfiguration : IEntityTypeConfiguration<DoctorRequest>
{
    public void Configure(EntityTypeBuilder<DoctorRequest> builder)
    {
        builder.HasOne(x => x.Team)
            .WithMany()
            .HasForeignKey(x => x.TeamId);

        builder.HasOne(x => x.Doctor)
            .WithMany(x => x.DoctorRequests)
            .HasForeignKey(x => x.DoctorId);
    }
}