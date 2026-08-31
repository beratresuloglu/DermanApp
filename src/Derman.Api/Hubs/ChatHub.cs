using System.Security.Claims;
using Derman.Api.Data;
using Derman.Core.Entities;
using Derman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Derman.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly AppDbContext _db;

    public ChatHub(AppDbContext db)
    {
        _db = db;
    }

    private Guid CurrentUserId =>
        Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User!.FindFirstValue("sub")!);

    public async Task JoinMatch(Guid matchId)
    {
        if (!await IsAuthorizedForMatch(matchId))
            throw new HubException("Bu eşleşmeye erişim yetkiniz yok.");

        await Groups.AddToGroupAsync(Context.ConnectionId, matchId.ToString());
    }

    public async Task SendMessage(Guid matchId, string content)
    {
        if (!await IsAuthorizedForMatch(matchId))
            throw new HubException("Bu eşleşmeye erişim yetkiniz yok.");

        var match = await _db.Matches.FindAsync(matchId);
        if (match is null || match.Status != MatchStatus.Onaylandi)
            throw new HubException("Mesajlaşma sadece onaylanmış eşleşmelerde mümkündür.");

        var receiverId = match.HelperUserId == CurrentUserId
            ? (await _db.HelpRequests.FindAsync(match.HelpRequestId))!.UserId
            : match.HelperUserId;

        var message = new Message
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            SenderId = CurrentUserId,
            ReceiverId = receiverId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        await Clients.Group(matchId.ToString()).SendAsync("ReceiveMessage", new
        {
            message.Id,
            message.SenderId,
            message.Content,
            message.SentAt
        });
    }

    private async Task<bool> IsAuthorizedForMatch(Guid matchId)
    {
        var match = await _db.Matches.FindAsync(matchId);
        if (match is null) return false;

        var request = await _db.HelpRequests.FindAsync(match.HelpRequestId);
        if (request is null) return false;

        return CurrentUserId == request.UserId || CurrentUserId == match.HelperUserId;
    }
}