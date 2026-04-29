using Evalify.Domain.Entities.Template;
using Evalify.Domain.Entities.TemplateQuestion;
using Evalify.Domain.Entities.StudentPaper;
using Evalify.Domain.Entities.StudentAnswer;
using Evalify.Domain.Entities.ProcessingJob;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Template> Templates { get; }
    DbSet<TemplateQuestion> TemplateQuestions { get; }
    DbSet<StudentPaper> StudentPapers { get; }
    DbSet<StudentAnswer> StudentAnswers { get; }
    DbSet<ProcessingJob> ProcessingJobs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
