using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Infrastructure.Data.Configurations;

public class PropertyOwnershipConfiguration : IEntityTypeConfiguration<PropertyOwnership>
{
    public void Configure(EntityTypeBuilder<PropertyOwnership> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasDefaultValueSql("NEWID()");
        builder.Property(o => o.EffectiveFrom).IsRequired();
        builder.Property(o => o.AcquisitionPrice).HasColumnType("decimal(18,2)");
        builder.Property(o => o.AcquisitionCurrency).IsRequired().HasMaxLength(3);

        // Composite index supports current-owner lookup (PropertyId + EffectiveTill == null).
        builder.HasIndex(o => new { o.PropertyId, o.EffectiveTill });
    }
}
