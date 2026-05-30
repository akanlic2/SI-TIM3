using ConferenceManagement.Application.DTOs.Report;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;
using QuestPdfDocument = QuestPDF.Fluent.Document;

namespace ConferenceManagement.Application.Services;

public class ConferenceReportService : IConferenceReportService
{
    private readonly IConferenceRepository _conferenceRepository;
    private readonly IConferenceRegistrationRepository _registrationRepository;
    private readonly ISessionRepository _sessionRepository;

    public ConferenceReportService(
        IConferenceRepository conferenceRepository,
        IConferenceRegistrationRepository registrationRepository,
        ISessionRepository sessionRepository)
    {
        _conferenceRepository = conferenceRepository;
        _registrationRepository = registrationRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<ConferenceReportDto> GetReportAsync(
        Guid conferenceId, CancellationToken cancellationToken)
    {
        var conference = await _conferenceRepository
            .GetByIdWithOrganizersAsync(conferenceId, cancellationToken)
            ?? throw new KeyNotFoundException("Konferencija nije pronađena.");

        var registrations = await _registrationRepository
            .GetRegistrationsByConferenceAsync(conferenceId, cancellationToken);

        var sessions = await _sessionRepository
            .GetSessionsByConferenceIdWithDetailsAsync(conferenceId, cancellationToken);

        var sessionReports = sessions.Select(s => new SessionReportDto
        {
            SessionId = s.SessionId,
            Title = s.Title,
            RegisteredCount = s.SessionRegistrations.Count,
            RoomCapacity = s.Room?.Capacity ?? 0,
            SpeakerCount = s.SessionRegistrations.Count(r => r.IsSpeaker),
            MaterialCount = s.Materials.Count
        }).ToList();

        return new ConferenceReportDto
        {
            ConferenceId = conference.ConferenceId,
            Title = conference.Title,
            Location = conference.Location,
            StartDate = conference.StartDate,
            EndDate = conference.EndDate,
            RegistrationStats = new RegistrationStatsDto
            {
                Total = registrations.Count,
                Confirmed = registrations.Count(r => r.RegistrationStatus == "Confirmed"),
                Pending = registrations.Count(r => r.RegistrationStatus == "Pending"),
                Cancelled = registrations.Count(r => r.RegistrationStatus == "Cancelled")
            },
            Sessions = sessionReports,
            TotalMaterials = sessions.Sum(s => s.Materials.Count),
            TotalSpeakers = sessions
                .SelectMany(s => s.SessionRegistrations)
                .Count(r => r.IsSpeaker)
        };
    }

    public async Task<byte[]> GenerateReportPdfAsync(
        Guid conferenceId, CancellationToken cancellationToken)
    {
        var report = await GetReportAsync(conferenceId, cancellationToken);

        QuestPDF.Settings.License = LicenseType.Community;

        return QuestPdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(t => t.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text(report.Title).FontSize(20).Bold();
                    col.Item().Text(
                        $"{report.Location} | " +
                        $"{report.StartDate:dd.MM.yyyy} – {report.EndDate:dd.MM.yyyy}")
                        .FontSize(11).FontColor(Colors.Grey.Medium);
                    col.Item().PaddingTop(4).LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(16).Column(col =>
                {
                    col.Item().Text("Statistike prijava").FontSize(14).Bold();
                    col.Item().PaddingTop(8).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            foreach (var label in new[]
                                { "Ukupno", "Potvrđeno", "Na čekanju", "Otkazano" })
                                h.Cell().Background(Colors.Grey.Lighten3)
                                    .Padding(6).Text(label).Bold();
                        });

                        foreach (var val in new[]
                        {
                            report.RegistrationStats.Total.ToString(),
                            report.RegistrationStats.Confirmed.ToString(),
                            report.RegistrationStats.Pending.ToString(),
                            report.RegistrationStats.Cancelled.ToString()
                        })
                            table.Cell().Padding(6).Text(val);
                    });

                    col.Item().PaddingTop(20).Text("Sesije").FontSize(14).Bold();
                    col.Item().PaddingTop(8).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3);
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            foreach (var label in new[]
                                { "Sesija", "Prijavljeni", "Kapacitet", "Predavači", "Materijali" })
                                h.Cell().Background(Colors.Grey.Lighten3)
                                    .Padding(6).Text(label).Bold();
                        });

                        foreach (var s in report.Sessions)
                        {
                            table.Cell().Padding(6).Text(s.Title);
                            table.Cell().Padding(6).Text(s.RegisteredCount.ToString());
                            table.Cell().Padding(6)
                                .Text(s.RoomCapacity > 0 ? s.RoomCapacity.ToString() : "—");
                            table.Cell().Padding(6).Text(s.SpeakerCount.ToString());
                            table.Cell().Padding(6).Text(s.MaterialCount.ToString());
                        }
                    });

                    col.Item().PaddingTop(20).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Ukupno predavača").Bold();
                            c.Item().Text(report.TotalSpeakers.ToString());
                        });
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Ukupno materijala").Bold();
                            c.Item().Text(report.TotalMaterials.ToString());
                        });
                    });
                });

                page.Footer().AlignRight().Text(t =>
                {
                    t.Span("Generisano: ");
                    t.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                        .FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }
}