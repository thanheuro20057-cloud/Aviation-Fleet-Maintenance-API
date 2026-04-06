using AviationFleet.Api.Models;

namespace AviationFleet.Api.Services;

public static class PartCertificationMapper
{
    public static CertificationType ToCertification(AircraftPartType part) =>
        part switch
        {
            AircraftPartType.Avionics => CertificationType.Avionics,
            AircraftPartType.Hydraulics => CertificationType.Hydraulics,
            AircraftPartType.LandingGear => CertificationType.Hydraulics,
            _ => CertificationType.Engine,
        };
}
