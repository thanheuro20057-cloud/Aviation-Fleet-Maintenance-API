using AviationFleet.Api.Models;

namespace AviationFleet.Api.Services;

public static class AircraftPartDisplay
{
    public static string Name(AircraftPartType type) =>
        type switch
        {
            AircraftPartType.Fuselage => "Fuselage / airframe",
            AircraftPartType.Engine => "Engine",
            AircraftPartType.LandingGear => "Landing gear",
            AircraftPartType.Avionics => "Avionics",
            AircraftPartType.Hydraulics => "Hydraulics",
            AircraftPartType.Apu => "APU",
            _ => type.ToString(),
        };
}
