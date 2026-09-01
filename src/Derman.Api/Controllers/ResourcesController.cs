using Derman.Api.Data;
using Derman.Api.DTOs;
using Derman.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Derman.Api.Controllers;

[ApiController]
[Route("api/resources")]
[Authorize]
public class ResourcesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ResourcesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby([FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] decimal radiusKm = 10)
    {
        var delta = radiusKm / 111m;

        var resources = await _db.Resources
            .Where(r => r.Latitude >= lat - delta && r.Latitude <= lat + delta)
            .Where(r => r.Longitude >= lng - delta && r.Longitude <= lng + delta)
            .ToListAsync();

        return Ok(resources.Select(ToDto));
    }

    private static ResourceResponseDto ToDto(Resource r) => new(
        r.Id, r.Name, r.Type, r.Address, r.Phone, r.Latitude, r.Longitude);
}