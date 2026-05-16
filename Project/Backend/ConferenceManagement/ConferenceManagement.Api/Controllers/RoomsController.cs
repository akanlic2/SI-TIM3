using ConferenceManagement.Application.DTOs.Room;
using ConferenceManagement.Dal;
using ConferenceManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/rooms")] 
public class RoomsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoomsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> GetAllRooms()
    {
        try
        {
            var rooms = await _context.Rooms
                .Select(r => new
                {
                    r.RoomId,
                    r.Name,
                    r.Location,
                    r.Capacity,
                    r.Description
                })
                .ToListAsync();

            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Greška pri dohvatanju dvorana.", details = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto roomDto)
    {
        try
        {
            var exists = await _context.Rooms
                .AnyAsync(r => r.Name.ToLower() == roomDto.Name.ToLower()
                            && r.Location.ToLower() == roomDto.Location.ToLower());

            if (exists)
            {
                return BadRequest(new { error = "Dvorana sa ovim nazivom već postoji na navedenoj lokaciji." });
            }

            var newRoom = new Room
            {
                RoomId = Guid.NewGuid(),
                Name = roomDto.Name,
                Location = roomDto.Location,
                Capacity = roomDto.Capacity,
                Description = roomDto.Description
            };

            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllRooms), new { id = newRoom.RoomId }, newRoom);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Greška pri kreiranju dvorane.", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] CreateRoomDto roomDto)
    {
        try
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound(new { error = "Dvorana nije pronađena." });
            }

            var duplicateExists = await _context.Rooms
                .AnyAsync(r => r.RoomId != id
                            && r.Name.ToLower() == roomDto.Name.ToLower()
                            && r.Location.ToLower() == roomDto.Location.ToLower());

            if (duplicateExists)
            {
                return BadRequest(new { error = "Druga dvorana sa ovim nazivom već postoji na ovoj lokaciji." });
            }

            room.Name = roomDto.Name;
            room.Location = roomDto.Location;
            room.Capacity = roomDto.Capacity;
            room.Description = roomDto.Description;

            await _context.SaveChangesAsync();

            return NoContent(); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Greška pri izmjeni dvorane.", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        try
        {
            var room = await _context.Rooms
                .Include(r => r.Sessions)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound(new { error = "Dvorana nije pronađena." });
            }

            if (room.Sessions.Any())
            {
                return BadRequest(new
                {
                    error = "Dvorana se ne može obrisati jer je dodijeljena sesijama.",
                    sessionCount = room.Sessions.Count
                });
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Greška pri brisanju dvorane.", details = ex.Message });
        }
    }
}