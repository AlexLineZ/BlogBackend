using BlogBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}