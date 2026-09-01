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
        builder.Entity<Resource>().HasData(
    new Resource
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Name = "Merkez Eczanesi",
        Type = "Eczane",
        Address = "Trabzon Mah. Atatürk Cad. No:12, Kahramanmaraş",
        Phone = "0344 123 45 67",
        Latitude = 37.5753m,
        Longitude = 36.9228m
    },
    new Resource
    {
        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Name = "Kahramanmaraş Devlet Hastanesi",
        Type = "Hastane",
        Address = "İsmet Paşa Mah. Hastane Cad. No:1, Kahramanmaraş",
        Phone = "0344 221 20 00",
        Latitude = 37.5822m,
        Longitude = 36.9337m
    },
    new Resource
    {
        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        Name = "AFAD Koordinasyon Merkezi",
        Type = "STK",
        Address = "Yörükselim Mah. Cumhuriyet Meydanı, Kahramanmaraş",
        Phone = "0344 225 10 10",
        Latitude = 37.5700m,
        Longitude = 36.9200m
    },
    new Resource
    {
        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
        Name = "Yeşilyurt Eczanesi",
        Type = "Eczane",
        Address = "Yeşilyurt Mah. No:45, Kahramanmaraş",
        Phone = "0344 234 56 78",
        Latitude = 37.5900m,
        Longitude = 36.9400m
    },
    new Resource
    {
        Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
        Name = "Kızılay Şube",
        Type = "STK",
        Address = "Fevzi Çakmak Mah. No:8, Kahramanmaraş",
        Phone = "0344 245 67 89",
        Latitude = 37.5650m,
        Longitude = 36.9150m
    }
);
    }
}