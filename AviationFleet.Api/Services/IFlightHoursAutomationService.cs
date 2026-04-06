using AviationFleet.Api.Models;

namespace AviationFleet.Api.Services;

public sealed record FlightHoursAutomationResult(
    bool ThresholdExceeded,
    bool AutoTicketCreated,
    string? AlertMessage);

public interface IFlightHoursAutomationService
{
    /// <summary>
    /// Applies part hour deltas, evaluates thresholds, updates operational status, may create an auto ticket when on ground.
    /// </summary>
    Task<FlightHoursAutomationResult> RunAfterTotalHoursUpdatedAsync(
        Aircraft aircraft,
        double previousTotalFlightHours,
        CancellationToken cancellationToken = default);
}
