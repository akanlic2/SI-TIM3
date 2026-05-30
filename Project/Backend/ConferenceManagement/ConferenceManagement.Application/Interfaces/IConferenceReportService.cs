using ConferenceManagement.Application.DTOs.Report;

namespace ConferenceManagement.Application.Interfaces;

public interface IConferenceReportService
{
    Task<ConferenceReportDto> GetReportAsync(Guid conferenceId, CancellationToken cancellationToken);
    Task<byte[]> GenerateReportPdfAsync(Guid conferenceId, CancellationToken cancellationToken);
}