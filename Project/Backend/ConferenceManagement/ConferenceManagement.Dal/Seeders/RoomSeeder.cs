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
                Capacity = 150
            },
            new Room
            {
                RoomId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Sala 203 (Lab)",
                Capacity = 30
            },
            new Room
            {
                RoomId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Konferencijska Sala A",
                Capacity = 50
            }
        };

        var roomsToAdd = new List<Room>();

        foreach (var room in seedRooms)
        {
            // Provjeravamo da li sala sa tim imenom već postoji da izbjegnemo duplikate
            var roomExists = await context.Rooms
                .Where(r => r.Name == room.Name)
                .FirstOrDefaultAsync(cancellationToken);

            if (roomExists == null)
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