using FireflyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FireflyAPI.Infrastructure.Configuration;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(r => r.Amount)
            .IsRequired();

        builder.HasIndex(r => r.ProjectId);
        
        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}