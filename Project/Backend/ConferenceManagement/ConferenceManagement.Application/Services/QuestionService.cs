using ConferenceManagement.Application.DTOs.Notification;
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
    private readonly INotificationService _notificationService;

    public QuestionService(
        IQuestionRepository questionRepository,
        ISessionRepository sessionRepository,
        IUserContextService userContextService,
        IUserRepository userRepository,
        INotificationService notificationService)
    {
        _questionRepository = questionRepository;
        _sessionRepository = sessionRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
        _notificationService = notificationService;
    }

    public async Task<QuestionDto> CreateQuestionAsync(
        Guid sessionId,
        CreateQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session is null)
            throw new KeyNotFoundException($"Sesija sa ID-jem {sessionId} nije pronađena.");

        if (session.StartTime > DateTime.UtcNow)
            throw new InvalidOperationException("Pitanja se mogu postavljati tek nakon početka sesije.");

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

        try
        {
            var fullSession = await _sessionRepository.GetByIdWithRegistrationsAsync(sessionId);
            var speaker = fullSession?.SessionRegistrations.FirstOrDefault(r => r.IsSpeaker);

            if (speaker is not null)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    UserId = speaker.UserId,
                    Title = "Novo pitanje u sesiji",
                    Content = $"Postavljeno je novo pitanje u sesiji \"{fullSession!.Title}\": \"{question.Content.Substring(0, Math.Min(question.Content.Length, 100))}\" [conferenceId:{fullSession.ConferenceId}]",
                    NotificationType = "QuestionAsked"
                }, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[NotificationError] CreateQuestion: {ex.Message}");
        }

        var author = await _userRepository.GetByIdAsync(userId);
        return MapToDto(question, author);
    }

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

    public async Task<QuestionDto> AnswerQuestionAsync(
        Guid sessionId,
        Guid questionId,
        AnswerQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Answer) && !dto.AnsweredOrally)
            throw new ArgumentException("Odgovor ne smije biti prazan.");

        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question is null)
            throw new KeyNotFoundException($"Pitanje sa ID-jem {questionId} nije pronađeno.");

        if (question.SessionId != sessionId)
            throw new ArgumentException("Pitanje ne pripada zadatoj sesiji.");

        var session = await _sessionRepository.GetByIdWithRegistrationsAsync(sessionId);
        if (session is null)
            throw new KeyNotFoundException($"Sesija sa ID-jem {sessionId} nije pronađena.");

        var userId = Guid.Parse(_userContextService.GetUserId());
        var isSpeaker = session.SessionRegistrations
            .Any(registration => registration.UserId == userId && registration.IsSpeaker);

        if (!isSpeaker)
            throw new UnauthorizedAccessException("Nemate dozvolu da odgovarate na pitanja za ovu sesiju.");

        question.Answer = dto.Answer?.Trim() ?? string.Empty;
        question.Status = "Answered";

        await _questionRepository.SaveChangesAsync();

        try
        {
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = question.UserId,
                Title = "Vaše pitanje je dobilo odgovor",
                Content = $"Predavač je odgovorio na vaše pitanje: \"{question.Content.Substring(0, Math.Min(question.Content.Length, 100))}\" [conferenceId:{session.ConferenceId}]",
                NotificationType = "QuestionAnswered"
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[NotificationError] AnswerQuestion: {ex.Message}");
        }

        var author = await _userRepository.GetByIdAsync(question.UserId);
        return MapToDto(question, author);
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