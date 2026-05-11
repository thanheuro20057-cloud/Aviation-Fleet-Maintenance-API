using AviationFleet.Api.Models;

namespace AviationFleet.Api.Services;

public static class AircraftPartDefaults
{
    public static IReadOnlyList<(AircraftPartType Type, double DefaultThresholdHours)> StandardKit { get; } =
    [
        (AircraftPartType.Fuselage, 6000),
        (AircraftPartType.WingStructure, 6500),
        (AircraftPartType.Empennage, 7000),
        (AircraftPartType.Engine, 2500),
        (AircraftPartType.EngineCompressor, 2200),
        (AircraftPartType.EngineCombustor, 1800),
        (AircraftPartType.EngineTurbine, 2000),
        (AircraftPartType.EngineFuelControl, 1500),
        (AircraftPartType.LandingGear, 8000),
        (AircraftPartType.NoseLandingGear, 7500),
        (AircraftPartType.BrakeAssembly, 1200),
        (AircraftPartType.WheelAndTire, 900),
        (AircraftPartType.Avionics, 4000),
        (AircraftPartType.FlightComputer, 3500),
        (AircraftPartType.NavigationSystem, 3000),
        (AircraftPartType.CommunicationSystem, 2800),
        (AircraftPartType.Transponder, 3200),
        (AircraftPartType.Hydraulics, 3500),
        (AircraftPartType.HydraulicReservoir, 4500),
        (AircraftPartType.HydraulicActuator, 2500),
        (AircraftPartType.HydraulicLines, 3000),
        (AircraftPartType.Apu, 3000),
        (AircraftPartType.ApuStarter, 1800),
        (AircraftPartType.ApuGenerator, 2400),
        (AircraftPartType.ApuFuelSystem, 2200),
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
