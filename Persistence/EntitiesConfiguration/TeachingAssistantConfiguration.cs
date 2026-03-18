using EduBridge.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduBridge.Persistence.EntitiesConfiguration;

public class TeachingAssistantConfiguration : IEntityTypeConfiguration<TeachingAssistant>
{
    public void Configure(EntityTypeBuilder<TeachingAssistant> builder)
    {
        builder.Property(x => x.Department).HasMaxLength(200).IsRequired();
        builder.Property(x => x.AcademicTitle).HasMaxLength(200);
        builder.Property(x => x.OfficeLocation).HasMaxLength(200);

        builder.HasOne(x => x.User)
            .WithOne(x => x.TeachingAssistant)
            .HasForeignKey<TeachingAssistant>(x => x.UserId);
    }
}