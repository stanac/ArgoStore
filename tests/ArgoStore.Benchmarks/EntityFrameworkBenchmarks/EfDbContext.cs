using ArgoStore.Benchmarks.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgoStore.Benchmarks.EntityFrameworkBenchmarks;

public class EfDbContext : DbContext
{
    private readonly string _connStr;

    public EfDbContext(string connStr)
    {
        _connStr = connStr;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connStr);
    }

    public DbSet<Person> Persons { get; set; } = null!;
}