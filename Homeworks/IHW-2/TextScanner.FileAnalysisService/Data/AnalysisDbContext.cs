using Microsoft.EntityFrameworkCore;
using TextScanner.FileAnalysisService.Models;

namespace TextScanner.FileAnalysisService.Data;

public class AnalysisDbContext : DbContext
{
    public DbSet<AnalysisResult> AnalysisResults { get; set; }

    public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalysisResult>(entity =>
        {
            entity.ToTable("AnalysisResults");
            entity.HasKey(e => e.FileId);
            entity.Property(e => e.FileId).IsRequired();
            entity.Property(e => e.ParagraphCount);
            entity.Property(e => e.WordCount);
            entity.Property(e => e.CharacterCount);
            entity.Property(e => e.Hash).IsRequired();
        });
    }
}