using AviationFleet.Api.Models;

namespace AviationFleet.Api.Services;

public static class AircraftPartDefaults
{
    public static IReadOnlyList<(AircraftPartType Type, double DefaultThresholdHours)> StandardKit { get; } =
    [
        (AircraftPartType.Fuselage, 6000),
        (AircraftPartType.Engine, 2500),
        (AircraftPartType.LandingGear, 8000),
        (AircraftPartType.Avionics, 4000),
        (AircraftPartType.Hydraulics, 3500),
        (AircraftPartType.Apu, 3000),
    ];

    public static void EnsureStandardParts(Aircraft aircraft, double initialHoursSinceService)
    {
        foreach (var (type, threshold) in StandardKit)
        {
            aircraft.Parts.Add(new AircraftPart
            {
                Id = Guid.NewGuid(),
                AircraftId = aircraft.Id,
                PartType = type,
                HoursSinceLastMaintenance = initialHoursSinceService,
                MaintenanceThresholdHours = threshold,
            });
        }
    }
}
