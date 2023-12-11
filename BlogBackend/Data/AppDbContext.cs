using BlogBackend.Models;
using BlogBackend.Models.Comments;
using BlogBackend.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Data;

public class AppDbContext: DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ExpiredTokenStorage> ExpiredTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}