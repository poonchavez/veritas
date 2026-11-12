using Microsoft.EntityFrameworkCore;
using Veritas.Web.Models;

namespace Veritas.Web.Data;

public sealed class IntentionsDb(DbContextOptions<IntentionsDb> options) : DbContext(options)
{
    public DbSet<Intention> Intentions => Set<Intention>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Intention>().ToTable("intentions").HasKey(i => i.Id);
        modelBuilder.Entity<Intention>().Property(i => i.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Intention>().HasIndex(i => new { i.Language, i.CreatedAt });
    }
}
