using System.Security.Claims;
using Derman.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Derman.Api.Controllers;

[ApiController]
[Route("api/matches/{matchId}/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _db;

    public MessagesController(AppDbContext db)
    {
        _db = db;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetMessages(Guid matchId)
    {
        var match = await _db.Matches.FindAsync(matchId);
        if (match is null) return NotFound();

        var request = await _db.HelpRequests.FindAsync(match.HelpRequestId);
        if (request is null) return NotFound();

        if (CurrentUserId != request.UserId && CurrentUserId != match.HelperUserId)
            return Forbid();

        var messages = await _db.Messages
            .Where(m => m.MatchId == matchId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Ok(messages);
    }
}