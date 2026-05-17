using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FireflyAPI.Infrastructure.Configuration;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");

        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Duration)
            .IsRequired();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(120);
        
        builder.HasIndex(a => a.ProjectId);
        
        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}