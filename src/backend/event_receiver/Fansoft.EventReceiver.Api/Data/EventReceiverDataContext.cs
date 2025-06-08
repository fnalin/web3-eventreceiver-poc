using Fansoft.EventReceiver.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fansoft.EventReceiver.Api.Data;

public class EventReceiverDataContext(DbContextOptions<EventReceiverDataContext> options) : DbContext(options)
{
    public DbSet<DigitalTwin> DigitalTwins => Set<DigitalTwin>();
    public DbSet<ApplicationProvider> ApplicationProviders => Set<ApplicationProvider>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var entityTypeDt = modelBuilder.Entity<DigitalTwin>();
        entityTypeDt.ToTable("DigitalTwins").HasKey(dt => dt.Id);
        entityTypeDt.Property(x => x.WalletId).HasMaxLength(64).IsRequired();
        entityTypeDt.Property(x => x.KeyName).HasMaxLength(3).IsRequired();
        entityTypeDt.Property(x => x.DtCallbackUrl).HasMaxLength(1024).IsRequired();
        
        var entityTypeApp = modelBuilder.Entity<ApplicationProvider>();
        entityTypeApp.ToTable("ApplicationProviders");
        entityTypeApp.HasKey(ap => ap.Id);
        entityTypeApp.Property(ap => ap.KeyName)
            .HasMaxLength(128)
            .IsRequired();
        entityTypeApp.Property(ap => ap.WalletId)
            .HasMaxLength(64)
            .IsRequired();
        entityTypeApp.Property(ap => ap.AppWebHookUrl)
            .HasMaxLength(1024);
        entityTypeApp.Property(ap => ap.AmqpHost)
            .HasMaxLength(256);
        entityTypeApp.Property(ap => ap.AmqpPort);
        entityTypeApp.Property(ap => ap.AmqpVirtualHost)
            .HasMaxLength(128)
            .HasDefaultValue("/");
        entityTypeApp.Property(ap => ap.AmqpUser)
            .HasMaxLength(128);
        entityTypeApp.Property(ap => ap.AmqpPassword)
            .HasMaxLength(128);
        entityTypeApp.Property(ap => ap.AmqpQueueName)
            .HasMaxLength(256);
        entityTypeApp.Property(ap => ap.EnableWebhook)
            .IsRequired();
        entityTypeApp.Property(ap => ap.EnableAmqp)
            .IsRequired();
        entityTypeApp.Property(ap => ap.IsActive)
            .IsRequired();
        entityTypeApp.Property(ap => ap.CreatedAt)
            .IsRequired();
    }
}