namespace Derman.Web.Models;

public record MessageDto(Guid Id, Guid SenderId, string Content, DateTime SentAt);