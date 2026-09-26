using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Transection> Transections {get; set;}
    public DbSet<User> Users {get; set;}
    public DbSet<Goal> Goals { get; set; }
}