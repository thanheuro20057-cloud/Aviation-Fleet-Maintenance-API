namespace AviationFleet.Api.Models;

/// <summary>
/// Ready: cleared for service. MaintenanceRequired: due/overdue. InMaintenance: ticket assigned.
/// Unavailable: major repair/overhaul (not routine line maintenance).
/// </summary>
public enum AircraftOperationalStatus
{
    Ready = 0,
    MaintenanceRequired = 1,
    InMaintenance = 2,
    Unavailable = 3,
}
