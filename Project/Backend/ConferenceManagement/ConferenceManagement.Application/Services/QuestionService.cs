using ConferenceManagement.Application.DTOs.Question;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;

    // S47-BE-03: INotificationService injection — implementira Osoba E
    // Kad Osoba E završi servis, dodaj: private readonly INotificationService _notificationService;
    // i poziv ispod u CreateQuestionAsync

    public QuestionService(
        IQuestionRepository questionRepository,
        ISessionRepository sessionRepository,
        IUserContextService userContextService,
        IUserRepository userRepository)
    {
        _questionRepository = questionRepository;
        _sessionRepository = sessionRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
    }

    // S47-BE-01
    public async Task<QuestionDto> CreateQuestionAsync(
        Guid sessionId,
        CreateQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        // Validacija: sesija mora postojati
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session is null)
            throw new KeyNotFoundException($"Sesija sa ID-jem {sessionId} nije pronađena.");

        // Validacija: sesija mora biti već počela (startTime <= now)
        if (session.StartTime > DateTime.UtcNow)
            throw new InvalidOperationException("Pitanja se mogu postavljati tek nakon početka sesije.");

        // Validacija: content nije prazan
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new ArgumentException("Sadržaj pitanja ne smije biti prazan.");

        if (dto.Content.Length > 500)
            throw new ArgumentException("Pitanje ne smije biti duže od 500 znakova.");

        var userId = Guid.Parse(_userContextService.GetUserId());

        var question = new Question
        {
            QuestionId = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = userId,
            Content = dto.Content.Trim(),
            AskedAt = DateTime.UtcNow,
            Status = "Open",
            Answer = string.Empty
        };

        await _questionRepository.AddAsync(question);
        await _questionRepository.SaveChangesAsync();

        // S47-BE-03: Emituj notifikacijski event prema servisu (Osoba E)
        // Event se šalje async — ne blokira 201 response
        // Kad INotificationService bude dostupan, uncommentaj:
        //
        // _ = Task.Run(async () =>
        // {
        //     var lecturer = await GetSessionLecturerAsync(session);
        //     if (lecturer is not null)
        //     {
        //         await _notificationService.SendAsync(new NewQuestionEvent
        //         {
        //             Event       = "NEW_QUESTION",
        //             SessionId   = sessionId,
        //             QuestionId  = question.QuestionId,
        //             QuestionText = question.Content,
        //             AuthorId    = userId,
        //             LecturerId  = lecturer.UserId,
        //             Timestamp   = question.AskedAt
        //         });
        //     }
        // }, CancellationToken.None);

        var author = await _userRepository.GetByIdAsync(userId);

        return MapToDto(question, author);
    }

    // S47-BE-02
    public async Task<List<QuestionDto>> GetQuestionsBySessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session is null)
            throw new KeyNotFoundException($"Sesija sa ID-jem {sessionId} nije pronađena.");

        var questions = await _questionRepository.GetBySessionIdAsync(sessionId);

        return questions.Select(q => MapToDto(q, q.User)).ToList();
    }

    private static QuestionDto MapToDto(Question question, User? author)
    {
        return new QuestionDto
        {
            QuestionId = question.QuestionId,
            SessionId = question.SessionId,
            UserId = question.UserId,
            AuthorName = author is not null
                ? $"{author.FirstName} {author.LastName}"
                : "Nepoznat korisnik",
            Content = question.Content,
            AskedAt = question.AskedAt,
            Answer = string.IsNullOrEmpty(question.Answer) ? null : question.Answer,
            Status = question.Status
        };
    }
}