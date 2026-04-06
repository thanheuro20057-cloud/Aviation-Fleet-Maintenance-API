using AviationFleet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Api.Data;

public class AviationFleetDbContext(DbContextOptions<AviationFleetDbContext> options) : DbContext(options)
{
    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<AircraftPart> AircraftParts => Set<AircraftPart>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<MaintenanceTicket> MaintenanceTickets => Set<MaintenanceTicket>();
    public DbSet<FleetSettings> FleetSettings => Set<FleetSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aircraft>(e =>
        {
            e.HasIndex(a => a.TailNumber).IsUnique();
            e.HasMany(a => a.Parts)
                .WithOne(p => p.Aircraft!)
                .HasForeignKey(p => p.AircraftId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Mechanic>(e =>
        {
            e.HasIndex(m => m.NameNormalized).IsUnique();
        });

        modelBuilder.Entity<MaintenanceTicket>(e =>
        {
            e.HasOne(t => t.Aircraft!)
                .WithMany(a => a.MaintenanceTickets)
                .HasForeignKey(t => t.AircraftId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(t => t.Mechanic)
                .WithMany(m => m.AssignedTickets)
                .HasForeignKey(t => t.MechanicId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FleetSettings>(e =>
        {
            e.HasKey(f => f.Id);
        });
    }
}
