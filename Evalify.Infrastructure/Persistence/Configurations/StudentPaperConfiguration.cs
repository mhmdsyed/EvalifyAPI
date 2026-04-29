using Evalify.Domain.Entities.StudentPaper;
using Evalify.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evalify.Infrastructure.Persistence.Configurations;

public sealed class StudentPaperConfiguration : IEntityTypeConfiguration<StudentPaper>
{
    public void Configure(EntityTypeBuilder<StudentPaper> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.StudentCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ImagePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<PaperStatus>(s));

        builder.Property(p => p.TotalGrade).IsRequired(false);
        builder.Property(p => p.CreatedAt).IsRequired();

        builder.HasMany(p => p.Answers)
            .WithOne(a => a.StudentPaper)
            .HasForeignKey(a => a.StudentPaperId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.ProcessingJob)
            .WithOne(j => j.StudentPaper)
            .HasForeignKey<Domain.Entities.ProcessingJob.ProcessingJob>(j => j.StudentPaperId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
