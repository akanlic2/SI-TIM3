using ConferenceManagement.Application.DTOs.Question;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class QuestionServiceTests
{
    private readonly Mock<IQuestionRepository> _questionRepositoryMock = new();
    private readonly Mock<ISessionRepository> _sessionRepositoryMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();

    private QuestionService CreateService() =>
        new(
            _questionRepositoryMock.Object,
            _sessionRepositoryMock.Object,
            _userContextMock.Object,
            _userRepositoryMock.Object,
            _notificationServiceMock.Object
        );

    [Fact]
    public async Task CreateQuestionAsync_SessionNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Session?)null);

        var dto = new CreateQuestionDto
        {
            Content = "Da li će materijali biti dostupni poslije sesije?"
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.CreateQuestionAsync(sessionId, dto));
    }

    [Fact]
    public async Task CreateQuestionAsync_SessionNotStarted_ThrowsInvalidOperationException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                StartTime = DateTime.UtcNow.AddHours(1)
            });

        var dto = new CreateQuestionDto
        {
            Content = "Pitanje prije početka sesije?"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateQuestionAsync(sessionId, dto));
    }

    [Fact]
    public async Task CreateQuestionAsync_EmptyContent_ThrowsArgumentException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                StartTime = DateTime.UtcNow.AddMinutes(-10)
            });

        var dto = new CreateQuestionDto
        {
            Content = "   "
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateQuestionAsync(sessionId, dto));
    }

    [Fact]
    public async Task CreateQuestionAsync_ContentLongerThan500_ThrowsArgumentException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                StartTime = DateTime.UtcNow.AddMinutes(-10)
            });

        var dto = new CreateQuestionDto
        {
            Content = new string('a', 501)
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateQuestionAsync(sessionId, dto));
    }

    [Fact]
    public async Task CreateQuestionAsync_ValidQuestion_CreatesQuestionAndNotifiesSpeaker()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var speakerId = Guid.NewGuid();
        var conferenceId = Guid.NewGuid();

        var dto = new CreateQuestionDto
        {
            Content = "   Da li će prezentacija biti dostupna poslije sesije?   "
        };

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(userId.ToString());

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                StartTime = DateTime.UtcNow.AddMinutes(-10)
            });

        _sessionRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                ConferenceId = conferenceId,
                Title = "AI sesija",
                SessionRegistrations = new List<SessionRegistration>
                {
                    new SessionRegistration
                    {
                        UserId = speakerId,
                        IsSpeaker = true,
                        RegistrationStatus = "Confirmed"
                    }
                }
            });

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "Ucesnik"
            });

        var result = await service.CreateQuestionAsync(sessionId, dto);

        Assert.NotNull(result);
        Assert.Equal(sessionId, result.SessionId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Test Ucesnik", result.AuthorName);
        Assert.Equal("Da li će prezentacija biti dostupna poslije sesije?", result.Content);
        Assert.Equal("Open", result.Status);
        Assert.Null(result.Answer);

        _questionRepositoryMock.Verify(r => r.AddAsync(It.Is<Question>(
            q => q.SessionId == sessionId &&
                 q.UserId == userId &&
                 q.Content == "Da li će prezentacija biti dostupna poslije sesije?" &&
                 q.Status == "Open"
        )), Times.Once);

        _questionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        _notificationServiceMock.Verify(n => n.CreateNotificationAsync(
            It.Is<ConferenceManagement.Application.DTOs.Notification.CreateNotificationDto>(
                notification =>
                    notification.UserId == speakerId &&
                    notification.Title == "Novo pitanje u sesiji" &&
                    notification.NotificationType == "QuestionAsked"
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetQuestionsBySessionAsync_SessionNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Session?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetQuestionsBySessionAsync(sessionId));
    }

    [Fact]
    public async Task GetQuestionsBySessionAsync_ExistingSession_ReturnsQuestions()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                StartTime = DateTime.UtcNow.AddMinutes(-10)
            });

        _questionRepositoryMock
            .Setup(r => r.GetBySessionIdAsync(sessionId))
            .ReturnsAsync(new List<Question>
            {
                new Question
                {
                    QuestionId = Guid.NewGuid(),
                    SessionId = sessionId,
                    UserId = userId,
                    Content = "Prvo pitanje",
                    Answer = "Odgovor",
                    Status = "Answered",
                    AskedAt = DateTime.UtcNow,
                    User = new User
                    {
                        UserId = userId,
                        FirstName = "Test",
                        LastName = "Ucesnik"
                    }
                }
            });

        var result = await service.GetQuestionsBySessionAsync(sessionId);

        Assert.Single(result);
        Assert.Equal("Prvo pitanje", result[0].Content);
        Assert.Equal("Odgovor", result[0].Answer);
        Assert.Equal("Answered", result[0].Status);
        Assert.Equal("Test Ucesnik", result[0].AuthorName);
    }

    [Fact]
    public async Task AnswerQuestionAsync_EmptyAnswerAndNotOral_ThrowsArgumentException()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        var dto = new AnswerQuestionDto
        {
            Answer = "   ",
            AnsweredOrally = false
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AnswerQuestionAsync(sessionId, questionId, dto));
    }

    [Fact]
    public async Task AnswerQuestionAsync_QuestionNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        _questionRepositoryMock
            .Setup(r => r.GetByIdAsync(questionId))
            .ReturnsAsync((Question?)null);

        var dto = new AnswerQuestionDto
        {
            Answer = "Odgovor na pitanje",
            AnsweredOrally = false
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AnswerQuestionAsync(sessionId, questionId, dto));
    }

    [Fact]
    public async Task AnswerQuestionAsync_QuestionDoesNotBelongToSession_ThrowsArgumentException()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var otherSessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        _questionRepositoryMock
            .Setup(r => r.GetByIdAsync(questionId))
            .ReturnsAsync(new Question
            {
                QuestionId = questionId,
                SessionId = otherSessionId,
                Content = "Pitanje"
            });

        var dto = new AnswerQuestionDto
        {
            Answer = "Odgovor",
            AnsweredOrally = false
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AnswerQuestionAsync(sessionId, questionId, dto));
    }

    [Fact]
    public async Task AnswerQuestionAsync_SessionNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        _questionRepositoryMock
            .Setup(r => r.GetByIdAsync(questionId))
            .ReturnsAsync(new Question
            {
                QuestionId = questionId,
                SessionId = sessionId,
                Content = "Pitanje"
            });

        _sessionRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(sessionId))
            .ReturnsAsync((Session?)null);

        var dto = new AnswerQuestionDto
        {
            Answer = "Odgovor",
            AnsweredOrally = false
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AnswerQuestionAsync(sessionId, questionId, dto));
    }

    [Fact]
    public async Task AnswerQuestionAsync_UserIsNotAssignedSpeaker_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var otherSpeakerId = Guid.NewGuid();

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(currentUserId.ToString());

        _questionRepositoryMock
            .Setup(r => r.GetByIdAsync(questionId))
            .ReturnsAsync(new Question
            {
                QuestionId = questionId,
                SessionId = sessionId,
                UserId = Guid.NewGuid(),
                Content = "Pitanje"
            });

        _sessionRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                SessionRegistrations = new List<SessionRegistration>
                {
                    new SessionRegistration
                    {
                        UserId = otherSpeakerId,
                        IsSpeaker = true
                    }
                }
            });

        var dto = new AnswerQuestionDto
        {
            Answer = "Odgovor",
            AnsweredOrally = false
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.AnswerQuestionAsync(sessionId, questionId, dto));
    }

    [Fact]
    public async Task AnswerQuestionAsync_AssignedSpeaker_AnswersQuestionAndNotifiesAuthor()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var speakerId = Guid.NewGuid();
        var conferenceId = Guid.NewGuid();

        var question = new Question
        {
            QuestionId = questionId,
            SessionId = sessionId,
            UserId = authorId,
            Content = "Koji su materijali dostupni?",
            Status = "Open",
            Answer = ""
        };

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(speakerId.ToString());

        _questionRepositoryMock
            .Setup(r => r.GetByIdAsync(questionId))
            .ReturnsAsync(question);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                ConferenceId = conferenceId,
                Title = "AI sesija",
                SessionRegistrations = new List<SessionRegistration>
                {
                    new SessionRegistration
                    {
                        UserId = speakerId,
                        IsSpeaker = true,
                        RegistrationStatus = "Confirmed"
                    }
                }
            });

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                UserId = authorId,
                FirstName = "Autor",
                LastName = "Pitanja"
            });

        var dto = new AnswerQuestionDto
        {
            Answer = "Materijali će biti dostupni nakon sesije.",
            AnsweredOrally = false
        };

        var result = await service.AnswerQuestionAsync(sessionId, questionId, dto);

        Assert.Equal("Answered", question.Status);
        Assert.Equal("Materijali će biti dostupni nakon sesije.", question.Answer);
        Assert.Equal("Answered", result.Status);
        Assert.Equal("Materijali će biti dostupni nakon sesije.", result.Answer);
        Assert.Equal("Autor Pitanja", result.AuthorName);

        _questionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        _notificationServiceMock.Verify(n => n.CreateNotificationAsync(
            It.Is<ConferenceManagement.Application.DTOs.Notification.CreateNotificationDto>(
                notification =>
                    notification.UserId == authorId &&
                    notification.Title == "Vaše pitanje je dobilo odgovor" &&
                    notification.NotificationType == "QuestionAnswered"
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}