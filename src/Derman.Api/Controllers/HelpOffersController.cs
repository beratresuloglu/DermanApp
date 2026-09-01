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
[Route("api/help-offers")]
[Authorize]
public class HelpOffersController : ControllerBase
{
    private readonly AppDbContext _db;

    public HelpOffersController(AppDbContext db)
    {
        _db = db;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);

    [HttpPost]
    [Authorize(Policy = "YardimciOnly")]
    public async Task<IActionResult> Create(CreateHelpOfferDto dto)
    {
        var offer = new HelpOffer
        {
            Id = Guid.NewGuid(),
            UserId = CurrentUserId,
            Category = dto.Category,
            Quantity = dto.Quantity,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Status = OfferStatus.Acik
        };

        _db.HelpOffers.Add(offer);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = offer.Id }, ToDto(offer));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var offer = await _db.HelpOffers.FindAsync(id);
        if (offer is null) return NotFound();
        return Ok(ToDto(offer));
    }

    [HttpGet("nearby")]
    [Authorize(Policy = "AfetzedeOnly")]
    public async Task<IActionResult> GetNearby([FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] decimal radiusKm = 10)
    {
        var delta = radiusKm / 111m;

        var offers = await _db.HelpOffers
            .Where(o => o.Status == OfferStatus.Acik)
            .Where(o => o.Latitude >= lat - delta && o.Latitude <= lat + delta)
            .Where(o => o.Longitude >= lng - delta && o.Longitude <= lng + delta)
            .ToListAsync();

        return Ok(offers.Select(o => ToDto(o, fuzzLocation: true)));
    }

    private static HelpOfferResponseDto ToDto(HelpOffer o, bool fuzzLocation = false)
    {
        var (lat, lng) = fuzzLocation
            ? Services.LocationFuzzer.Fuzz(o.Latitude, o.Longitude)
            : (o.Latitude, o.Longitude);

        return new(o.Id, o.Category, o.Quantity, o.Status.ToString(), lat, lng, o.CreatedAt);
    }
}