using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Infrastructure.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("NEWID()");
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.PhoneNumber).HasMaxLength(50);
        builder.Property(c => c.EmailAddress).IsRequired().HasMaxLength(255);

        builder.HasMany(c => c.Ownerships)
            .WithOne(o => o.Contact)
            .HasForeignKey(o => o.ContactId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
