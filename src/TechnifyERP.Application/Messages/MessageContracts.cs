namespace TechnifyERP.Application.Messages;
public sealed record ContactDto(string UserId,string FullName,string Email,string Role);
public sealed record MessageDto(int Id,string SenderUserId,string SenderName,string RecipientUserId,string RecipientName,string Subject,string Body,DateTime SentAtUtc,DateTime? ReadAtUtc);
public enum MessageActionResult{Success,NotFound,Forbidden,Invalid}
public interface IMessageService
{
 Task<IReadOnlyList<ContactDto>>GetContactsAsync(string userId,string role,CancellationToken ct=default);
 Task<IReadOnlyList<MessageDto>>GetInboxAsync(string userId,CancellationToken ct=default);
 Task<IReadOnlyList<MessageDto>>GetSentAsync(string userId,CancellationToken ct=default);
 Task<MessageDto?>GetAsync(string userId,int id,CancellationToken ct=default);
 Task<MessageActionResult>SendAsync(string senderId,string senderRole,string recipientId,string subject,string body,CancellationToken ct=default);
}
