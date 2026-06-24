using Microsoft.EntityFrameworkCore;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Infrastructure.Data.Seed;

/// <summary>
/// Seeds the baseline data described in the functional specification (§5).
/// Idempotent: does nothing if properties already exist.
/// </summary>
public static class DataSeeder
{
    // Deterministic GUIDs keep relationships stable across reseeds and make the data easy to reference.
    private static readonly Guid CarmenId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid JoshuaId = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid JoeId = new("33333333-3333-3333-3333-333333333333");

    private static readonly Guid MaisonetteId = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid PenthouseId = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Properties.AnyAsync())
            return;

        var contacts = new[]
        {
            new Contact { Id = CarmenId, FirstName = "Carmen", LastName = "Attard", PhoneNumber = "+356 99001122", EmailAddress = "carmen@example.com" },
            new Contact { Id = JoshuaId, FirstName = "Joshua", LastName = "Mifsud", PhoneNumber = "+356 99003344", EmailAddress = "joshua@example.com" },
            new Contact { Id = JoeId,    FirstName = "Joe",    LastName = "Borg",   PhoneNumber = "+356 99005566", EmailAddress = "joe@example.com" }
        };
        await db.Contacts.AddRangeAsync(contacts);

        var registration = new DateTime(2023, 1, 1);

        var maisonette = new Property
        {
            Id = MaisonetteId,
            Name = "Maisonette",
            Address = "12 Triq il-Kbira, Birkirkara",
            Price = 130000m,
            Currency = "EUR",
            DateOfRegistration = registration
        };
        var penthouse = new Property
        {
            Id = PenthouseId,
            Name = "Penthouse",
            Address = "3 Triq San Pawl, Valletta",
            Price = 430000m,
            Currency = "EUR",
            DateOfRegistration = registration
        };
        await db.Properties.AddRangeAsync(maisonette, penthouse);

        // Initial price-history record per property (registration price).
        await db.PropertyPriceHistories.AddRangeAsync(
            new PropertyPriceHistory { Id = Guid.NewGuid(), PropertyId = MaisonetteId, Amount = 130000m, Currency = "EUR", EffectiveDate = registration },
            new PropertyPriceHistory { Id = Guid.NewGuid(), PropertyId = PenthouseId, Amount = 430000m, Currency = "EUR", EffectiveDate = registration });

        // Ownership history (matches the dashboard example).
        await db.PropertyOwnerships.AddRangeAsync(
            // Maisonette: Joshua (past) -> Carmen (current)
            new PropertyOwnership
            {
                Id = Guid.NewGuid(),
                PropertyId = MaisonetteId,
                ContactId = JoshuaId,
                EffectiveFrom = new DateTime(2023, 7, 25),
                EffectiveTill = new DateTime(2024, 1, 15), // closed when Carmen took over
                AcquisitionPrice = 100000m,
                AcquisitionCurrency = "EUR"
            },
            new PropertyOwnership
            {
                Id = Guid.NewGuid(),
                PropertyId = MaisonetteId,
                ContactId = CarmenId,
                EffectiveFrom = new DateTime(2024, 1, 15),
                EffectiveTill = null, // current owner
                AcquisitionPrice = 120000m,
                AcquisitionCurrency = "EUR"
            },
            // Penthouse: Joe (current)
            new PropertyOwnership
            {
                Id = Guid.NewGuid(),
                PropertyId = PenthouseId,
                ContactId = JoeId,
                EffectiveFrom = new DateTime(2023, 5, 6),
                EffectiveTill = null,
                AcquisitionPrice = 400000m,
                AcquisitionCurrency = "EUR"
            });

        await db.SaveChangesAsync();
    }
}
