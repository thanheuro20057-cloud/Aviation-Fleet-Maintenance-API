using AviationFleet.Api.Data;
using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;
using AviationFleet.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Tests;

public sealed class DispatcherIntegrationTests
{
    private static AviationFleetDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AviationFleetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var ctx = new AviationFleetDbContext(options);
        ctx.Database.EnsureCreated();
        ctx.FleetSettings.Add(new FleetSettings { Id = 1, MaxMechanicActiveJobs = 10 });
        ctx.SaveChanges();
        return ctx;
    }

    private static Mechanic M(string name, CertificationType c, bool avail, int load) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        NameNormalized = name.ToLowerInvariant(),
        Certification = c,
        IsAvailable = avail,
        CurrentWorkload = load,
    };

    [Fact]
    public async Task Dispatcher_IgnoresUnavailableMechanics()
    {
        await using var db = CreateContext();

        db.Mechanics.Add(M("U1", CertificationType.Hydraulics, false, 0));
        await db.SaveChangesAsync();

        var aircraft = new Aircraft
        {
            Id = Guid.NewGuid(),
            TailNumber = "N1",
            TotalFlightHours = 0,
            MaintenanceThresholdHours = 100,
            HoursAtLastMaintenance = 0,
            LocationState = AircraftLocationState.OnGround,
            OperationalStatus = AircraftOperationalStatus.Ready,
        };
        AircraftPartDefaults.EnsureStandardParts(aircraft, 0);
        db.Aircraft.Add(aircraft);
        await db.SaveChangesAsync();

        var ticketRepo = new MaintenanceTicketRepository(db);
        var mechanicRepo = new MechanicRepository(db);
        var unit = new UnitOfWork(db);
        var fleet = new FleetSettingsRepository(db);
        var dispatcher = new DispatcherService(mechanicRepo, fleet);
        var aircraftRepo = new AircraftRepository(db);

        var maintenance = new MaintenanceService(aircraftRepo, ticketRepo, dispatcher, unit);
        var created = await maintenance.CreateTicketAsync(
            new AviationFleet.Api.Dtos.CreateMaintenanceTicketDto(aircraft.Id, "Leak", CertificationType.Hydraulics));

        Assert.NotNull(created);
        Assert.Null(created.MechanicId);
        Assert.Equal(MaintenanceTicketStatus.Open, created.Status);
    }

    [Fact]
    public async Task Dispatcher_IgnoresMismatchedCertification()
    {
        await using var db = CreateContext();

        db.Mechanics.Add(M("Av1", CertificationType.Avionics, true, 0));
        await db.SaveChangesAsync();

        var aircraft = new Aircraft
        {
            Id = Guid.NewGuid(),
            TailNumber = "N2",
            TotalFlightHours = 0,
            MaintenanceThresholdHours = 100,
            HoursAtLastMaintenance = 0,
            LocationState = AircraftLocationState.OnGround,
            OperationalStatus = AircraftOperationalStatus.Ready,
        };
        AircraftPartDefaults.EnsureStandardParts(aircraft, 0);
        db.Aircraft.Add(aircraft);
        await db.SaveChangesAsync();

        var ticketRepo = new MaintenanceTicketRepository(db);
        var mechanicRepo = new MechanicRepository(db);
        var unit = new UnitOfWork(db);
        var fleet = new FleetSettingsRepository(db);
        var dispatcher = new DispatcherService(mechanicRepo, fleet);
        var aircraftRepo = new AircraftRepository(db);

        var maintenance = new MaintenanceService(aircraftRepo, ticketRepo, dispatcher, unit);
        var created = await maintenance.CreateTicketAsync(
            new AviationFleet.Api.Dtos.CreateMaintenanceTicketDto(aircraft.Id, "Hyd issue", CertificationType.Hydraulics));

        Assert.Null(created!.MechanicId);
    }

    [Fact]
    public async Task Dispatcher_PicksLowestWorkload()
    {
        await using var db = CreateContext();

        var high = M("Zoe", CertificationType.Engine, true, 5);
        var low = M("Alex", CertificationType.Engine, true, 1);
        db.Mechanics.AddRange(high, low);
        await db.SaveChangesAsync();

        var aircraft = new Aircraft
        {
            Id = Guid.NewGuid(),
            TailNumber = "N3",
            TotalFlightHours = 0,
            MaintenanceThresholdHours = 100,
            HoursAtLastMaintenance = 0,
            LocationState = AircraftLocationState.OnGround,
            OperationalStatus = AircraftOperationalStatus.Ready,
        };
        AircraftPartDefaults.EnsureStandardParts(aircraft, 0);
        db.Aircraft.Add(aircraft);
        await db.SaveChangesAsync();

        var ticketRepo = new MaintenanceTicketRepository(db);
        var mechanicRepo = new MechanicRepository(db);
        var unit = new UnitOfWork(db);
        var fleet = new FleetSettingsRepository(db);
        var dispatcher = new DispatcherService(mechanicRepo, fleet);
        var aircraftRepo = new AircraftRepository(db);

        var maintenance = new MaintenanceService(aircraftRepo, ticketRepo, dispatcher, unit);
        var created = await maintenance.CreateTicketAsync(
            new AviationFleet.Api.Dtos.CreateMaintenanceTicketDto(aircraft.Id, "Engine", CertificationType.Engine));

        Assert.Equal(low.Id, created!.MechanicId);
        var reloaded = await db.Mechanics.FindAsync(low.Id);
        Assert.Equal(2, reloaded!.CurrentWorkload);
    }
}
