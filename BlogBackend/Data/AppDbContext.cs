using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}