using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FireflyAPI.Infrastructure.Configuration;

public class ActivityDependencyConfiguration : IEntityTypeConfiguration<ActivityDependency>
{
    public void Configure(EntityTypeBuilder<ActivityDependency> builder)
    {
       builder.ToTable("ActivityDependency");

       builder.HasKey(ad => new
       { 
           ad.ActivityId,
           ad.PredecessorActivityId
       });
       
       builder.HasOne<Activity>()
           .WithMany()
           .HasForeignKey(ad => ad.ActivityId)
           .OnDelete(DeleteBehavior.Cascade);
       
       builder.HasOne<Activity>()
           .WithMany()
           .HasForeignKey(ad => ad.PredecessorActivityId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}