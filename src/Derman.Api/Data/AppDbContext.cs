using Derman.Api.Identity;
using Derman.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Derman.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<HelpRequest> HelpRequests => Set<HelpRequest>();
    public DbSet<HelpOffer> HelpOffers => Set<HelpOffer>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Resource> Resources => Set<Resource>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // HelpRequest - Match ilişkisi (1 talep, birden fazla eşleşme denemesi olabilir)
        builder.Entity<Match>()
            .HasOne<HelpRequest>()
            .WithMany()
            .HasForeignKey(m => m.HelpRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Match - Message ilişkisi (mesajlar bir eşleşme bağlamında geçer)
        builder.Entity<Message>()
            .HasOne<Match>()
            .WithMany()
            .HasForeignKey(m => m.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Karar tablosu: Enum'ları veritabanında okunabilir string olarak sakla
        builder.Entity<ApplicationUser>()
            .Property(u => u.Role)
            .HasConversion<string>();

        builder.Entity<HelpRequest>()
            .Property(r => r.Status)
            .HasConversion<string>();

        builder.Entity<HelpRequest>()
            .Property(r => r.SuggestedUrgency)
            .HasConversion<string>();

        builder.Entity<HelpOffer>()
            .Property(o => o.Status)
            .HasConversion<string>();

        builder.Entity<Match>()
            .Property(m => m.Status)
            .HasConversion<string>();

        builder.Entity<Report>()
            .Property(r => r.Status)
            .HasConversion<string>();
    }
}