using AviationFleet.Api.Data;
using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;
using AviationFleet.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Tests;

public sealed class PredictiveMaintenanceServiceIntegrationTests
{
    [Fact]
    public async Task GetWarningRoster_ReturnsAircraftWithin36HourWindow()
    {
        var options = new DbContextOptionsBuilder<AviationFleetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new AviationFleetDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var aircraft = new Aircraft
        {
            Id = Guid.NewGuid(),
            TailNumber = "WARN",
            TotalFlightHours = 470,
            MaintenanceThresholdHours = 500,
            HoursAtLastMaintenance = 0,
            LocationState = AircraftLocationState.OnGround,
            OperationalStatus = AircraftOperationalStatus.Ready,
        };
        AircraftPartDefaults.EnsureStandardParts(aircraft, 0);
        db.Aircraft.Add(aircraft);
        await db.SaveChangesAsync();

        var repo = new AircraftRepository(db);
        var sut = new PredictiveMaintenanceService(repo);
        var roster = await sut.GetWarningRosterAsync();

        var w = Assert.Single(roster);
        Assert.Equal("WARN", w.TailNumber);
        Assert.Equal(30, w.HoursRemainingUntilThreshold);
    }
}
