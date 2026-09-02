using Microsoft.EntityFrameworkCore;using TechnifyERP.Application.Messages;using TechnifyERP.Domain.Entities;using TechnifyERP.Domain.Enums;using TechnifyERP.Infrastructure.Persistence;
namespace TechnifyERP.Infrastructure.Messages;
internal sealed class MessageService(ApplicationDbContext db):IMessageService
{
 public async Task<IReadOnlyList<ContactDto>>GetContactsAsync(string id,string role,CancellationToken ct=default)
 {
  IQueryable<string> ids=role switch {"Student"=>db.FacultyCourses.Where(x=>db.CourseEnrollments.Any(e=>e.StudentUserId==id&&e.CourseId==x.CourseId&&e.Status==EnrollmentStatus.Approved)).Select(x=>x.FacultyUserId),"Faculty"=>db.CourseEnrollments.Where(e=>e.Status==EnrollmentStatus.Approved&&db.FacultyCourses.Any(f=>f.FacultyUserId==id&&f.CourseId==e.CourseId)).Select(e=>e.StudentUserId),_=>db.Users.Where(x=>x.Id!=id&&x.AccountStatus==AccountStatus.Active).Select(x=>x.Id)};
  return await db.Users.AsNoTracking().Where(x=>ids.Contains(x.Id)).OrderBy(x=>x.FullName).Select(x=>new ContactDto(x.Id,x.FullName,x.Email??"",x.UserType.ToString())).Distinct().ToListAsync(ct);
 }
 public async Task<IReadOnlyList<MessageDto>>GetInboxAsync(string id,CancellationToken ct=default)=>await Project(db.PrivateMessages.AsNoTracking().Where(x=>x.RecipientUserId==id).OrderByDescending(x=>x.SentAtUtc)).ToListAsync(ct);
 public async Task<IReadOnlyList<MessageDto>>GetSentAsync(string id,CancellationToken ct=default)=>await Project(db.PrivateMessages.AsNoTracking().Where(x=>x.SenderUserId==id).OrderByDescending(x=>x.SentAtUtc)).ToListAsync(ct);
 public async Task<MessageDto?>GetAsync(string id,int msg,CancellationToken ct=default){var item=await db.PrivateMessages.FirstOrDefaultAsync(x=>x.Id==msg&&(x.SenderUserId==id||x.RecipientUserId==id),ct);if(item is null)return null;if(item.RecipientUserId==id&&item.ReadAtUtc is null){item.ReadAtUtc=DateTime.UtcNow;await db.SaveChangesAsync(ct);}return await Project(db.PrivateMessages.AsNoTracking().Where(x=>x.Id==msg)).FirstAsync(ct);}
 public async Task<MessageActionResult>SendAsync(string sender,string role,string recipient,string subject,string body,CancellationToken ct=default){if(string.IsNullOrWhiteSpace(recipient)||string.IsNullOrWhiteSpace(subject)||string.IsNullOrWhiteSpace(body))return MessageActionResult.Invalid;var contacts=await GetContactsAsync(sender,role,ct);if(!contacts.Any(c=>c.UserId==recipient))return MessageActionResult.Forbidden;db.Add(new PrivateMessage{SenderUserId=sender,RecipientUserId=recipient,Subject=subject.Trim(),Body=body.Trim()});await db.SaveChangesAsync(ct);return MessageActionResult.Success;}
 private IQueryable<MessageDto>Project(IQueryable<PrivateMessage>q)=>from x in q join s in db.Users.AsNoTracking() on x.SenderUserId equals s.Id join r in db.Users.AsNoTracking() on x.RecipientUserId equals r.Id select new MessageDto(x.Id,x.SenderUserId,s.FullName,x.RecipientUserId,r.FullName,x.Subject,x.Body,x.SentAtUtc,x.ReadAtUtc);
}
