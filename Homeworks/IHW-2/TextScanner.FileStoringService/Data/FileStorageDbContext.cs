using Microsoft.EntityFrameworkCore;
using TextScanner.FileStoringService.Models;

namespace TextScanner.FileStoringService.Data;

public class FileStorageDbContext : DbContext
{
    public DbSet<FileMetadata> FileMetadatas { get; set; }

    public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options) : base(options)
    {
    }
}