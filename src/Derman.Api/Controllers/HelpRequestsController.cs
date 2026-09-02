using System.Security.Claims;
using Derman.Api.Data;
using Derman.Api.DTOs;
using Derman.Api.Services;
using Derman.Core.Entities;
using Derman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Derman.Api.Controllers;

[ApiController]
[Route("api/help-requests")]
[Authorize]
public class HelpRequestsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAiTriageService _triageService;

    private readonly IAiPriorityService _priorityService;

    public HelpRequestsController(AppDbContext db, IAiTriageService triageService, IAiPriorityService priorityService)
    {
        _db = db;
        _triageService = triageService;
        _priorityService = priorityService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);

    [HttpPost]
    [Authorize(Policy = "AfetzedeOnly")]
    public async Task<IActionResult> Create(CreateHelpRequestDto dto)
    {
        var currentUser = await _db.Users.FindAsync(CurrentUserId);
        if (currentUser is Derman.Api.Identity.ApplicationUser appUser && appUser.IsBlocked)
            return Forbid();

        var request = new HelpRequest
        {
            Id = Guid.NewGuid(),
            UserId = CurrentUserId,
            Category = dto.Category,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Status = RequestStatus.Acik
        };

        var (urgency, reasoning) = await _triageService.ScoreUrgencyAsync(dto.Description);
        request.SuggestedUrgency = urgency;
        request.UrgencyReasoning = reasoning;

        _db.HelpRequests.Add(request);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = request.Id }, ToDto(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var request = await _db.HelpRequests.FindAsync(id);
        if (request is null) return NotFound();
        return Ok(ToDto(request));
    }

    [HttpGet("nearby")]
    [Authorize(Policy = "YardimciOnly")]
    public async Task<IActionResult> GetNearby([FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] decimal radiusKm = 10)
    {
        // Basit bir "kutu" filtresi: gerçek çember mesafesi değil, MVP için yeterli bir yaklaşım
        var delta = radiusKm / 111m; // yaklaşık 1 derece enlem ~111km

        var requests = await _db.HelpRequests
            .Where(r => r.Status == RequestStatus.Acik)
            .Where(r => r.Latitude >= lat - delta && r.Latitude <= lat + delta)
            .Where(r => r.Longitude >= lng - delta && r.Longitude <= lng + delta)
            .ToListAsync();

        return Ok(requests.Select(r => ToDto(r, fuzzLocation: true)));
    }

    [HttpGet("nearby/priority")]
    [Authorize(Policy = "YardimciOnly")]
    public async Task<IActionResult> GetNearbyWithPriority([FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] decimal radiusKm = 10)
    {
        var delta = radiusKm / 111m;

        var requests = await _db.HelpRequests
            .Where(r => r.Status == RequestStatus.Acik)
            .Where(r => r.Latitude >= lat - delta && r.Latitude <= lat + delta)
            .Where(r => r.Longitude >= lng - delta && r.Longitude <= lng + delta)
            .ToListAsync();

        var (summary, priorityIds) = await _priorityService.AnalyzeRegionAsync(requests);

        return Ok(new
        {
            Summary = summary,
            PriorityRequestIds = priorityIds,
            Requests = requests.Select(r => ToDto(r, fuzzLocation: true))
        });
    }

    [HttpPut("{id}/status")]
    [Authorize(Policy = "AfetzedeOnly")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] RequestStatus status)
    {
        var request = await _db.HelpRequests.FindAsync(id);
        if (request is null) return NotFound();

        if (request.UserId != CurrentUserId)
            return Forbid();

        request.Status = status;
        await _db.SaveChangesAsync();

        return Ok(ToDto(request));
    }

    [HttpGet("mine")]
    [Authorize(Policy = "AfetzedeOnly")]
    public async Task<IActionResult> GetMine()
    {
        var requests = await _db.HelpRequests
            .Where(r => r.UserId == CurrentUserId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(requests.Select(r => ToDto(r)));
    }
    private static HelpRequestResponseDto ToDto(HelpRequest r, bool fuzzLocation = false)
    {
        var (lat, lng) = fuzzLocation
            ? LocationFuzzer.Fuzz(r.Latitude, r.Longitude)
            : (r.Latitude, r.Longitude);

        return new(
            r.Id, r.Category, r.Description,
            r.SuggestedUrgency?.ToString(), r.UrgencyReasoning,
            r.Status.ToString(), lat, lng, r.CreatedAt);
    }
    [HttpGet("{id}/owner")]
    public async Task<IActionResult> GetOwner(Guid id)
    {
        var request = await _db.HelpRequests.FindAsync(id);
        if (request is null) return NotFound();

        // Sadece bu talebin eşleşmesinde yer alan biri (Afetzede kendisi veya eşleşen Yardımcı) bu bilgiyi görebilir
        var hasMatch = await _db.Matches.AnyAsync(m =>
            m.HelpRequestId == id && (m.HelperUserId == CurrentUserId || request.UserId == CurrentUserId));

        if (!hasMatch) return Forbid();

        return Ok(request.UserId);
    }
}