using ConferenceManagement.Application.DTOs.Agenda;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services;

public class AgendaItemService : IAgendaItemService
{
    private static readonly string[] SessionTypes = ["Session"];
    private static readonly string[] ValidTypes =
        ["Session", "Break", "Lunch", "Networking", "Opening", "Closing"];

    private readonly IAgendaItemRepository _agendaItemRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IConferenceRepository _conferenceRepository;

    public AgendaItemService(
        IAgendaItemRepository agendaItemRepository,
        ISessionRepository sessionRepository,
        IConferenceRepository conferenceRepository)
    {
        _agendaItemRepository = agendaItemRepository;
        _sessionRepository = sessionRepository;
        _conferenceRepository = conferenceRepository;
    }

    public async Task<List<AgendaItemDto>> GetByConferenceIdAsync(Guid conferenceId)
    {
        var items = await _agendaItemRepository.GetByConferenceIdAsync(conferenceId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<AgendaItemDto> CreateAsync(Guid conferenceId, CreateAgendaItemDto dto)
    {
        var conference = await _conferenceRepository.GetByIdAsync(conferenceId);
        if (conference is null)
            throw new KeyNotFoundException($"Konferencija sa ID-jem {conferenceId} nije pronađena.");

        ValidateType(dto.Type);

        var startLocal = AssumeLocal(dto.StartTime);
        var endLocal = AssumeLocal(dto.EndTime);
        ValidateTime(startLocal, endLocal);

        var startUtc = startLocal.ToUniversalTime();
        var endUtc = endLocal.ToUniversalTime();
        ValidateWithinConferenceTime(conference, startUtc, endUtc);

        string title = dto.Title;
        string description = dto.Description;
        Guid? resolvedSessionId = null;

        if (IsSessionType(dto.Type))
        {
            if (dto.SessionId is null)
                throw new ArgumentException("SessionId je obavezan za tip 'Session'.");

            var session = await _sessionRepository.GetByIdAsync(dto.SessionId.Value);
            if (session is null)
                throw new KeyNotFoundException($"Sesija sa ID-jem {dto.SessionId} nije pronađena.");

            ValidateMatchesSessionTime(session, startUtc, endUtc);

            title = session.Title;
            description = session.Description;
            resolvedSessionId = session.SessionId;
        }

        else
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Naziv je obavezan za ovaj tip agenda stavke.");
        }

        var agendaItem = new AgendaItem
        {
            AgendaItemId = Guid.NewGuid(),
            ConferenceId = conferenceId,
            SessionId = resolvedSessionId,
            RoomId = dto.RoomId,
            Title = title,
            Description = description,
            StartTime = startUtc,
            EndTime = endUtc,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };

        await _agendaItemRepository.AddAsync(agendaItem);
        await _agendaItemRepository.SaveChangesAsync();

        return MapToDto(agendaItem);
    }

    public async Task UpdateAsync(Guid agendaItemId, UpdateAgendaItemDto dto)
    {
        var agendaItem = await _agendaItemRepository.GetByIdAsync(agendaItemId);
        if (agendaItem is null)
            throw new KeyNotFoundException($"Agenda stavka sa ID-jem {agendaItemId} nije pronađena.");

        var conference = await _conferenceRepository.GetByIdAsync(agendaItem.ConferenceId);
        if (conference is null)
            throw new KeyNotFoundException($"Konferencija sa ID-jem {agendaItem.ConferenceId} nije pronađena.");

        ValidateType(dto.Type);

        var startLocal = AssumeLocal(dto.StartTime);
        var endLocal = AssumeLocal(dto.EndTime);
        ValidateTime(startLocal, endLocal);

        var startUtc = startLocal.ToUniversalTime();
        var endUtc = endLocal.ToUniversalTime();
        ValidateWithinConferenceTime(conference, startUtc, endUtc);

        string title = dto.Title;
        string description = dto.Description;
        Guid? resolvedSessionId = null;

        if (IsSessionType(dto.Type))
        {
            if (dto.SessionId is null)
                throw new ArgumentException("SessionId je obavezan za tip 'Session'.");

            var session = await _sessionRepository.GetByIdAsync(dto.SessionId.Value);
            if (session is null)
                throw new KeyNotFoundException($"Sesija sa ID-jem {dto.SessionId} nije pronađena.");

            ValidateMatchesSessionTime(session, startUtc, endUtc);

            title = session.Title;
            description = session.Description;
            resolvedSessionId = session.SessionId;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Naziv je obavezan za ovaj tip agenda stavke.");
        }

        agendaItem.Type = dto.Type;
        agendaItem.StartTime = startUtc;
        agendaItem.EndTime = endUtc;
        agendaItem.SessionId = resolvedSessionId;
        agendaItem.Title = title;
        agendaItem.Description = description;
        agendaItem.RoomId = dto.RoomId;

        await _agendaItemRepository.UpdateAsync(agendaItem);
        await _agendaItemRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid agendaItemId)
    {
        var agendaItem = await _agendaItemRepository.GetByIdAsync(agendaItemId);
        if (agendaItem is null)
            throw new KeyNotFoundException($"Agenda stavka sa ID-jem {agendaItemId} nije pronađena.");

        await _agendaItemRepository.DeleteAsync(agendaItem);
        await _agendaItemRepository.SaveChangesAsync();
    }

    private static void ValidateType(string type)
    {
        if (!ValidTypes.Contains(type))
            throw new ArgumentException($"Nepoznat tip agenda stavke: '{type}'. Dozvoljeni tipovi: {string.Join(", ", ValidTypes)}.");
    }

    private static void ValidateTime(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("Vrijeme završetka mora biti nakon vremena početka.");
    }

    private static void ValidateWithinConferenceTime(Conference conference, DateTime startUtc, DateTime endUtc)
    {
        var conferenceStartUtc = AssumeUtc(conference.StartDate);
        var conferenceEndUtc = AssumeUtc(conference.EndDate);

        if (startUtc < conferenceStartUtc || endUtc > conferenceEndUtc)
            throw new ArgumentException("Stavka agende mora biti unutar vremena konferencije.");
    }

    private static void ValidateMatchesSessionTime(Session session, DateTime startUtc, DateTime endUtc)
    {
        var sessionStartUtc = AssumeUtc(session.StartTime);
        var sessionEndUtc = AssumeUtc(session.EndTime);

        if (startUtc != sessionStartUtc || endUtc != sessionEndUtc)
            throw new ArgumentException("Vrijeme stavke agende mora tačno odgovarati vremenu sesije.");
    }

    private static DateTime AssumeLocal(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Local => value,
            DateTimeKind.Utc => value.ToLocalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Local)
        };
    }

    private static DateTime AssumeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private static bool IsSessionType(string type)
        => SessionTypes.Contains(type, StringComparer.OrdinalIgnoreCase);

    private static AgendaItemDto MapToDto(AgendaItem item)
    {
        var startLocal = AssumeUtc(item.StartTime).ToLocalTime();
        var endLocal = AssumeUtc(item.EndTime).ToLocalTime();
        var createdLocal = AssumeUtc(item.CreatedAt).ToLocalTime();

        return new AgendaItemDto
        {
            AgendaItemId = item.AgendaItemId,
            ConferenceId = item.ConferenceId,
            SessionId = item.SessionId,
            RoomId = item.RoomId,
            Title = item.Title,
            Description = item.Description,
            StartTime = startLocal,
            EndTime = endLocal,
            Type = item.Type,
            CreatedAt = createdLocal,
            SessionTitle = item.Session?.Title,
            SessionType = item.Session?.SessionType,
            SpeakerName = item.Session?.SessionRegistrations
                .FirstOrDefault(r => r.IsSpeaker)?.User != null
                ? $"{item.Session.SessionRegistrations.First(r => r.IsSpeaker).User.FirstName} {item.Session.SessionRegistrations.First(r => r.IsSpeaker).User.LastName}"
                : null,
            RoomName = item.Room?.Name
        };
    }
}