using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Infrastructure.Data.Configurations;

public class PropertyPriceHistoryConfiguration : IEntityTypeConfiguration<PropertyPriceHistory>
{
    public void Configure(EntityTypeBuilder<PropertyPriceHistory> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).HasDefaultValueSql("NEWID()");
        builder.Property(h => h.Amount).HasColumnType("decimal(18,2)");
        builder.Property(h => h.Currency).IsRequired().HasMaxLength(3);
        builder.Property(h => h.EffectiveDate).IsRequired();

        builder.HasIndex(h => h.PropertyId);
    }
}
