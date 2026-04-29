using Evalify.Domain.Entities.StudentAnswer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evalify.Infrastructure.Persistence.Configurations;

public sealed class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
{
    public void Configure(EntityTypeBuilder<StudentAnswer> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Grade).IsRequired();
        builder.Property(a => a.ExtractedText).IsRequired(false);
        builder.Property(a => a.CreatedAt).IsRequired();
    }
}
