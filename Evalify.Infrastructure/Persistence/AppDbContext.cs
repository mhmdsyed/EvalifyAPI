using Evalify.Application.Common.Interfaces;
using Evalify.Domain.Entities.ProcessingJob;
using Evalify.Domain.Entities.StudentAnswer;
using Evalify.Domain.Entities.StudentPaper;
using Evalify.Domain.Entities.Template;
using Evalify.Domain.Entities.TemplateQuestion;
using Evalify.Domain.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User>(options), IAppDbContext
{
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<TemplateQuestion> TemplateQuestions => Set<TemplateQuestion>();
    public DbSet<StudentPaper> StudentPapers => Set<StudentPaper>();
    public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();
    public DbSet<ProcessingJob> ProcessingJobs => Set<ProcessingJob>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
