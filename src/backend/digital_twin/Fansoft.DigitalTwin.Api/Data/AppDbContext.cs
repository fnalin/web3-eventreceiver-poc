using Fansoft.DigitalTwin.Api.Data.EfConfigurations;
using Fansoft.DigitalTwin.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.DigitalTwin.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StoredEventConfiguration());
    }
}