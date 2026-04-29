using Evalify.Domain.Entities.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evalify.Infrastructure.Persistence.Configurations;

public sealed class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.ImagePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.Width).IsRequired();
        builder.Property(t => t.Height).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasMany(t => t.Questions)
            .WithOne(q => q.Template)
            .HasForeignKey(q => q.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.StudentPapers)
            .WithOne(p => p.Template)
            .HasForeignKey(p => p.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
