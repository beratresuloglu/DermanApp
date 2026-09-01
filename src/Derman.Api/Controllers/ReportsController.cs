using System.Security.Claims;
using Derman.Api.Data;
using Derman.Api.DTOs;
using Derman.Api.Identity;
using Derman.Core.Entities;
using Derman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Derman.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;
    private const int BlockThreshold = 3;

    public ReportsController(AppDbContext db)
    {
        _db = db;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateReportDto dto)
    {
        if (dto.ReportedUserId == CurrentUserId)
            return BadRequest(new { message = "Kendinizi şikayet edemezsiniz." });

        var alreadyReported = await _db.Reports
            .AnyAsync(r => r.ReporterId == CurrentUserId && r.ReportedUserId == dto.ReportedUserId);

        if (alreadyReported)
            return BadRequest(new { message = "Bu kullanıcıyı zaten şikayet ettiniz." });

        var report = new Report
        {
            Id = Guid.NewGuid(),
            ReporterId = CurrentUserId,
            ReportedUserId = dto.ReportedUserId,
            Reason = dto.Reason,
            Status = ReportStatus.Beklemede
        };

        _db.Reports.Add(report);
        await _db.SaveChangesAsync();

        await CheckAndBlockIfThresholdExceeded(dto.ReportedUserId);

        return Ok(ToDto(report));
    }

    private async Task CheckAndBlockIfThresholdExceeded(Guid userId)
    {
        var reportCount = await _db.Reports.CountAsync(r => r.ReportedUserId == userId);

        if (reportCount >= BlockThreshold)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is ApplicationUser appUser && !appUser.IsBlocked)
            {
                appUser.IsBlocked = true;
                await _db.SaveChangesAsync();
            }
        }
    }

    private static ReportResponseDto ToDto(Report r) => new(
        r.Id, r.ReporterId, r.ReportedUserId, r.Reason, r.Status.ToString(), r.CreatedAt);
}