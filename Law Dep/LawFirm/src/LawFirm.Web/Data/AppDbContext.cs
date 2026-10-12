using LawFirm.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Web.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Lawyer> Lawyers => Set<Lawyer>();
    public DbSet<PracticeArea> PracticeAreas => Set<PracticeArea>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Lawyer>()
            .HasMany(l => l.PracticeAreas)
            .WithMany(p => p.Lawyers);

        builder.Entity<Appointment>(e =>
        {
            e.HasOne(a => a.Lawyer)
                .WithMany()
                .HasForeignKey(a => a.LawyerId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(a => a.PracticeArea)
                .WithMany()
                .HasForeignKey(a => a.PracticeAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Store the enum as readable text ("Pending") instead of 0/1/2.
            e.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);

            // Database-level guard against double booking: one active appointment
            // per lawyer per time slot. Cancelled appointments free the slot.
            e.HasIndex(a => new { a.LawyerId, a.PreferredDate })
                .IsUnique()
                .HasFilter("[LawyerId] IS NOT NULL AND [Status] <> 'Cancelled'");
        });
    }
}
