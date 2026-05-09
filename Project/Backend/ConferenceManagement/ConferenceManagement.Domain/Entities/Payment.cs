namespace ConferenceManagement.Domain.Entities;

public class Payment
{
    public Guid PaymentId { get; set; }
    public Guid ConferenceRegistrationId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;

    public ConferenceRegistration ConferenceRegistration { get; set; }
}