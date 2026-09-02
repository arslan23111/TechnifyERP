namespace TechnifyERP.Domain.Entities;
public sealed class PrivateMessage
{
    public int Id { get; set; }
    public string SenderUserId { get; set; } = string.Empty;
    public string RecipientUserId { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAtUtc { get; set; }
}
