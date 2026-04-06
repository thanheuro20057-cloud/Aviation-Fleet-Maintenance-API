using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AviationFleet.Api.Models;

public class AircraftPart
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid AircraftId { get; set; }

    [ForeignKey(nameof(AircraftId))]
    public Aircraft? Aircraft { get; set; }

    public AircraftPartType PartType { get; set; }

    /// <summary>Flight hours accumulated since last maintenance on this component.</summary>
    public double HoursSinceLastMaintenance { get; set; }

    public double MaintenanceThresholdHours { get; set; }
}
