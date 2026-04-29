using Evalify.Domain.Entities.TemplateQuestion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evalify.Infrastructure.Persistence.Configurations;

public sealed class TemplateQuestionConfiguration : IEntityTypeConfiguration<TemplateQuestion>
{
    public void Configure(EntityTypeBuilder<TemplateQuestion> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.ModelAnswer)
            .IsRequired();

        builder.Property(q => q.Mark).IsRequired();
        builder.Property(q => q.QuestionIndex).IsRequired();
        builder.Property(q => q.X).IsRequired();
        builder.Property(q => q.Y).IsRequired();
        builder.Property(q => q.Width).IsRequired();
        builder.Property(q => q.Height).IsRequired();

        builder.HasMany(q => q.StudentAnswers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
