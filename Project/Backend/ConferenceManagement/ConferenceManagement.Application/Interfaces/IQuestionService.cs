using ConferenceManagement.Application.DTOs.Question;

namespace ConferenceManagement.Application.Interfaces;

public interface IQuestionService
{
    /// <summary>
    /// S47-BE-01: Kreira novo pitanje za sesiju.
    /// Dozvoljeno samo prijavljenim korisnicima i samo ako je sesija već počela.
    /// </summary>
    Task<QuestionDto> CreateQuestionAsync(Guid sessionId, CreateQuestionDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// S47-BE-02: Vraća listu svih pitanja za sesiju, sortiranu po vremenu postavljanja.
    /// Uključuje odgovore predavača ako postoje.
    /// </summary>
    Task<List<QuestionDto>> GetQuestionsBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// S47-BE-03: Predavač odgovara na pitanje.
    /// Dozvoljeno samo predavaču dodijeljenom toj sesiji.
    /// </summary>
    Task<QuestionDto> AnswerQuestionAsync(Guid sessionId, Guid questionId, AnswerQuestionDto dto, CancellationToken cancellationToken = default);
}