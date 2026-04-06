namespace AviationFleet.Api.Services;

/// <summary>
/// Predictive window: 72 calendar hours at 12 flight hours/day ⇒ 36 flight hours before due.
/// Uses hours since last airframe maintenance.
/// </summary>
public static class PredictiveMaintenanceRules
{
    public const double WarningWindowFlightHours = 36;

    public static double HoursSinceLastAirframeMaintenance(double totalFlightHours, double hoursAtLastMaintenance) =>
        totalFlightHours - hoursAtLastMaintenance;

    public static double HoursRemainingUntilMaintenanceDue(
        double totalFlightHours,
        double hoursAtLastMaintenance,
        double maintenanceThresholdHours)
    {
        var since = HoursSinceLastAirframeMaintenance(totalFlightHours, hoursAtLastMaintenance);
        return maintenanceThresholdHours - since;
    }

    public static bool IsOnWarningRoster(
        double totalFlightHours,
        double hoursAtLastMaintenance,
        double maintenanceThresholdHours) =>
        HoursRemainingUntilMaintenanceDue(totalFlightHours, hoursAtLastMaintenance, maintenanceThresholdHours)
        <= WarningWindowFlightHours;
}
