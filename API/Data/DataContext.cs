using API.Entities;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{

    public DbSet<Greeting> Greetings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seeding data for the 'Product' entity
        modelBuilder.Entity<Greeting>().HasData(
            new Greeting { Id = 1, Name = "default", Greetings = "Hello, World!" }
        );

        base.OnModelCreating(modelBuilder);
    }

}