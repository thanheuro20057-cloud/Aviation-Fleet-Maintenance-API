using System.ComponentModel.DataAnnotations;

namespace AviationFleet.Api.Models;

/// <summary>Singleton row (Id = 1) for fleet-wide operational limits.</summary>
public class FleetSettings
{
    [Key]
    public int Id { get; set; } = 1;

    /// <summary>Mechanics will not receive new assignments when at or above this count.</summary>
    [Range(1, 100)]
    public int MaxMechanicActiveJobs { get; set; } = 3;
}
