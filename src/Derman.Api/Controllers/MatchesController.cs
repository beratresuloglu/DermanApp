using System.Security.Claims;
using Derman.Api.Data;
using Derman.Api.DTOs;
using Derman.Core.Entities;
using Derman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Derman.Api.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly AppDbContext _db;

    public MatchesController(AppDbContext db)
    {
        _db = db;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);

    [HttpPost]
    [Authorize(Policy = "YardimciOnly")]
    public async Task<IActionResult> Create(CreateMatchDto dto)
    {
        var currentUser = await _db.Users.FindAsync(CurrentUserId);
        if (currentUser is Derman.Api.Identity.ApplicationUser appUser && appUser.IsBlocked)
            return Forbid();

        var request = await _db.HelpRequests.FindAsync(dto.HelpRequestId);
        if (request is null) return NotFound("Talep bulunamadı.");
        if (request.Status != RequestStatus.Acik) return BadRequest("Bu talep artık açık değil.");

        var match = new Match
        {
            Id = Guid.NewGuid(),
            HelpRequestId = dto.HelpRequestId,
            HelperUserId = CurrentUserId,
            Status = MatchStatus.Bekliyor
        };

        _db.Matches.Add(match);
        request.Status = RequestStatus.OnayBekliyor;
        await _db.SaveChangesAsync();

        return Ok(ToDto(match));
    }

    [HttpPut("{id}/confirm")]
    [Authorize(Policy = "AfetzedeOnly")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var match = await _db.Matches.FindAsync(id);
        if (match is null) return NotFound();

        var request = await _db.HelpRequests.FindAsync(match.HelpRequestId);
        if (request is null || request.UserId != CurrentUserId) return Forbid();

        match.Status = MatchStatus.Onaylandi;
        match.ConfirmedAt = DateTime.UtcNow;
        request.Status = RequestStatus.Ustlenildi;

        await _db.SaveChangesAsync();
        return Ok(ToDto(match));
    }

    [HttpPut("{id}/reject")]
    [Authorize(Policy = "AfetzedeOnly")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var match = await _db.Matches.FindAsync(id);
        if (match is null) return NotFound();

        var request = await _db.HelpRequests.FindAsync(match.HelpRequestId);
        if (request is null || request.UserId != CurrentUserId) return Forbid();

        match.Status = MatchStatus.Reddedildi;
        request.Status = RequestStatus.Acik; // Talep tekrar açık hale gelir, başka Yardımcı üstlenebilir

        await _db.SaveChangesAsync();
        return Ok(ToDto(match));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var match = await _db.Matches.FindAsync(id);
        if (match is null) return NotFound();

        // Sadece eşleşmenin iki tarafı bu detayı görebilir
        var request = await _db.HelpRequests.FindAsync(match.HelpRequestId);
        if (request is null) return NotFound();
        if (CurrentUserId != request.UserId && CurrentUserId != match.HelperUserId)
            return Forbid();

        // Onaylanmışsa tam konum, değilse bulanık konum döndürmek istersen
        // burada HelpRequestsController'daki ToDto benzeri bir mantık eklenebilir.
        // Şimdilik Match detayında konum yok, sadece durum bilgisi dönüyoruz.

        return Ok(ToDto(match));
    }

    [HttpGet("by-request/{requestId}")]
    [Authorize(Policy = "AfetzedeOnly")]
    public async Task<IActionResult> GetByRequest(Guid requestId)
    {
        var request = await _db.HelpRequests.FindAsync(requestId);
        if (request is null || request.UserId != CurrentUserId) return Forbid();

        var match = await _db.Matches
            .Where(m => m.HelpRequestId == requestId)
            .OrderByDescending(m => m.RequestedAt)
            .FirstOrDefaultAsync();

        if (match is null) return NotFound();

        return Ok(ToDto(match));
    }
    private static MatchResponseDto ToDto(Match m) => new(
        m.Id, m.HelpRequestId, m.HelperUserId,
        m.Status.ToString(), m.RequestedAt, m.ConfirmedAt);

    [HttpGet("mine")]
    [Authorize(Policy = "YardimciOnly")]
    public async Task<IActionResult> GetMine()
    {
        var matches = await _db.Matches
            .Where(m => m.HelperUserId == CurrentUserId)
            .OrderByDescending(m => m.RequestedAt)
            .ToListAsync();

        return Ok(matches.Select(ToDto));
    }
}