namespace AviationFleet.Api.Dtos;

public sealed record BusinessDashboardDto(
    DateTime GeneratedAtUtc,
    BusinessKpiDto Kpis,
    IReadOnlyList<ResourceAllocationDto> ResourceAllocation,
    IReadOnlyList<BusinessInsightDto> Insights,
    IReadOnlyList<TrendSignalDto> Trends,
    IReadOnlyList<ActionRecommendationDto> Actions,
    IReadOnlyList<BusinessAssumptionDto> Assumptions);

public sealed record BusinessKpiDto(
    int AircraftCount,
    int ReadyAircraft,
    double FleetAvailabilityPercent,
    int OpenTickets,
    int UnassignedTickets,
    double CrewUtilizationPercent,
    int PredictiveWarnings,
    double EstimatedDowntimeHoursProtected,
    decimal DirectOperatingCostExposure,
    decimal MaintenanceCostAtRisk,
    decimal AogExposureProtected,
    decimal EstimatedMonthlySavings);

public sealed record ResourceAllocationDto(
    string Certification,
    int AvailableMechanics,
    int ActiveJobs,
    int OpenBacklog,
    int UnassignedBacklog,
    int CapacityRemaining,
    double UtilizationPercent,
    string Recommendation);

public sealed record BusinessInsightDto(
    string Severity,
    string Title,
    string Detail,
    string BusinessImpact);

public sealed record TrendSignalDto(
    string Name,
    string Direction,
    double Value,
    string Unit,
    string Detail);

public sealed record ActionRecommendationDto(
    int Priority,
    string Action,
    string Owner,
    string ExpectedOutcome);

public sealed record BusinessAssumptionDto(
    string Name,
    decimal Value,
    string Unit,
    string Source,
    string SourceUrl);
