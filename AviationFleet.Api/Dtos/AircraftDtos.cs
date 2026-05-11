using AviationFleet.Api.Models;

namespace AviationFleet.Api.Dtos;

public sealed record AircraftPartResponseDto(
    Guid Id,
    string PartCode,
    AircraftPartType PartType,
    string PartName,
    double HoursSinceLastMaintenance,
    double MaintenanceThresholdHours);

public sealed record AircraftCreateDto(
    string TailNumber,
    double TotalFlightHours,
    double MaintenanceThresholdHours,
    double HoursAtLastMaintenance,
    AircraftLocationState LocationState,
    AircraftOperationalStatus OperationalStatus);

public sealed record AircraftResponseDto(
    Guid Id,
    string TailNumber,
    double TotalFlightHours,
    double MaintenanceThresholdHours,
    double HoursAtLastMaintenance,
    AircraftLocationState LocationState,
    AircraftOperationalStatus OperationalStatus,
    IReadOnlyList<AircraftPartResponseDto> Parts);

public sealed record UpdateAircraftFlightHoursDto(double TotalFlightHours);

public sealed record UpdateFlightHoursResultDto(
    AircraftResponseDto Aircraft,
    bool ThresholdExceeded,
    bool AutoTicketCreated,
    string? AlertMessage);

public sealed record UpdateAircraftLocationDto(AircraftLocationState LocationState);

public sealed record UpdateAircraftOperationalStatusDto(AircraftOperationalStatus OperationalStatus);
