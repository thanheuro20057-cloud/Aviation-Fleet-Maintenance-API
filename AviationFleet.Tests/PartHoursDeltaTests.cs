using AviationFleet.Api.Data;
using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;
using AviationFleet.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Tests;

public sealed class PartHoursDeltaTests
{
    [Fact]
    public async Task DecreasingTotalFlightHours_ReducesPartHoursSinceService()
    {
        var options = new DbContextOptionsBuilder<AviationFleetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new AviationFleetDbContext(options);
        await db.Database.EnsureCreatedAsync();
        db.FleetSettings.Add(new FleetSettings { Id = 1, MaxMechanicActiveJobs = 10 });
        await db.SaveChangesAsync();

        var aircraft = new Aircraft
        {
            Id = Guid.NewGuid(),
            TailNumber = "NDELTA",
            TotalFlightHours = 200,
            MaintenanceThresholdHours = 500,
            HoursAtLastMaintenance = 0,
            LocationState = AircraftLocationState.OnGround,
            OperationalStatus = AircraftOperationalStatus.Ready,
        };
        AircraftPartDefaults.EnsureStandardParts(aircraft, 200);
        db.Aircraft.Add(aircraft);
        await db.SaveChangesAsync();

        var aircraftRepo = new AircraftRepository(db);
        var ticketRepo = new MaintenanceTicketRepository(db);
        var mechanicRepo = new MechanicRepository(db);
        var unit = new UnitOfWork(db);
        var automation = new FlightHoursAutomationService(ticketRepo, new DispatcherService(mechanicRepo, new FleetSettingsRepository(db)));

        var svc = new AircraftService(aircraftRepo, ticketRepo, mechanicRepo, automation, unit);
        await svc.UpdateFlightHoursAsync(aircraft.Id, new AviationFleet.Api.Dtos.UpdateAircraftFlightHoursDto(150));

        var reloaded = await db.Aircraft.Include(a => a.Parts).FirstAsync(a => a.Id == aircraft.Id);
        var engine = reloaded.Parts.First(p => p.PartType == AircraftPartType.Engine);
        Assert.Equal(150, engine.HoursSinceLastMaintenance, 5);
    }
}
