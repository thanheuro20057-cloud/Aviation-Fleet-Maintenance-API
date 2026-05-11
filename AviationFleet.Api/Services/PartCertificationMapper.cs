using AviationFleet.Api.Models;

namespace AviationFleet.Api.Services;

public static class PartCertificationMapper
{
    public static CertificationType ToCertification(AircraftPartType part) =>
        part switch
        {
            AircraftPartType.Avionics
                or AircraftPartType.FlightComputer
                or AircraftPartType.NavigationSystem
                or AircraftPartType.CommunicationSystem
                or AircraftPartType.Transponder => CertificationType.Avionics,
            AircraftPartType.Hydraulics
                or AircraftPartType.HydraulicReservoir
                or AircraftPartType.HydraulicActuator
                or AircraftPartType.HydraulicLines
                or AircraftPartType.LandingGear
                or AircraftPartType.NoseLandingGear
                or AircraftPartType.BrakeAssembly
                or AircraftPartType.WheelAndTire => CertificationType.Hydraulics,
            _ => CertificationType.Engine,
        };
}
