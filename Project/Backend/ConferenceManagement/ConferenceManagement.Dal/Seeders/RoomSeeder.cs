using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Seeders;

public static class RoomSeeder
{
    public static async Task SeedRoomsAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        var seedRooms = new List<Room>
        {
            new Room
{
                RoomId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Amfiteatar 1",
                Capacity = 150,
                Location = "ETF Sarajevo",
                Description = "Glavni amfiteatar"
            },
            new Room
            {
                RoomId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Sala 203 (Lab)",
                Capacity = 30,
                Location = "ETF Sarajevo",
                Description = "Laboratorijska sala sa računarima"
            },
            new Room
            {
                RoomId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Konferencijska Sala A",
                Capacity = 50,
                Location = "Hotel Hills",
                Description = "Sala za manje konferencije i radionice"
            }
        };

        var roomsToAdd = new List<Room>();

        foreach (var room in seedRooms)
        {
            var exists = await context.Rooms
                .AnyAsync(r => r.RoomId == room.RoomId, cancellationToken);

            if (!exists)
            {
                roomsToAdd.Add(room);
            }
        }

        if (roomsToAdd.Count > 0)
        {
            await context.Rooms.AddRangeAsync(roomsToAdd, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}