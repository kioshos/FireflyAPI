using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FireflyAPI.Infrastructure.Configuration;

public class ResourceRequirementConfiguration : IEntityTypeConfiguration<ResourceRequirement>
{
    public void Configure(EntityTypeBuilder<ResourceRequirement> builder)
    {
        builder.ToTable("ResourceRequirements");

        builder.HasKey(rr => new
        {
            rr.ActivityId,
            rr.ResourceId,
        });

        builder.Property(rr => rr.Amount)
            .IsRequired();
        
        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(rr => rr.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Resource>()
            .WithMany()
            .HasForeignKey(rr => rr.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}