using Fansoft.AppProvider.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.AppProvider.Api.Data;

public class EventProcessDataContext (DbContextOptions<EventProcessDataContext> options) : DbContext(options)
{
    public DbSet<EventProcess> EventProcesses => Set<EventProcess>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entityType = modelBuilder.Entity<EventProcess>();
        entityType.ToTable("EventProcesses").HasKey(ep => ep.Id);
        entityType.Property(ep => ep.EventHash)
            .HasMaxLength(66)
            .IsRequired();
        entityType.Property(ep => ep.Status)
            .IsRequired()
            .HasConversion<string>();
        entityType.Property(ep => ep.OriginalPayload)
            .IsRequired(false);
        entityType.Property(ep => ep.FailureReason)
            .IsRequired(false);
        entityType.Property(ep => ep.CreatedAt)
            .IsRequired();
    }
}