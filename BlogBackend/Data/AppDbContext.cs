using BlogBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<TokenStorage> Tokens { get; set; }
    public DbSet<Community> Communities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}