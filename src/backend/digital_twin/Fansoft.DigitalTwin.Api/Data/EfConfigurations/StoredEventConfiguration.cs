using Fansoft.DigitalTwin.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fansoft.DigitalTwin.Api.Data.EfConfigurations;

public  class StoredEventConfiguration : IEntityTypeConfiguration<StoredEvent>
{
    public void Configure(EntityTypeBuilder<StoredEvent> builder)
    {
        builder.ToTable("stored_event");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder.Property(o => o.ExternalId)
            .HasColumnName("external_id")
            .HasMaxLength(66)
            .IsRequired();
        
        builder.Property(o => o.OriginalPayload)
            .HasColumnName("original_payload")
            .HasColumnType("json")
            .IsRequired();
        
        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}