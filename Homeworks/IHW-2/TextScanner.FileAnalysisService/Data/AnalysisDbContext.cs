using Microsoft.EntityFrameworkCore;
using TextScanner.FileAnalysisService.Models;

namespace TextScanner.FileAnalysisService.Data
{
    public class AnalysisDbContext : DbContext
    {
        public DbSet<AnalysisResult> AnalysisResults { get; set; }

        public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnalysisResult>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd(); // Указываем, что Id автоинкрементируется
        }
    }
}