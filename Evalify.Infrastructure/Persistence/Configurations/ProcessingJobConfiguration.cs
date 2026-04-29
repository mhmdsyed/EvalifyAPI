using Evalify.Domain.Entities.ProcessingJob;
using Evalify.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evalify.Infrastructure.Persistence.Configurations;

public sealed class ProcessingJobConfiguration : IEntityTypeConfiguration<ProcessingJob>
{
    public void Configure(EntityTypeBuilder<ProcessingJob> builder)
    {
        builder.HasKey(j => j.Id);

        builder.HasIndex(j => j.StudentPaperId).IsUnique();

        builder.Property(j => j.Status)
            .IsRequired()
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<JobStatus>(s));

        builder.Property(j => j.CreatedAt).IsRequired();
        builder.Property(j => j.CompletedAt).IsRequired(false);
    }
}
