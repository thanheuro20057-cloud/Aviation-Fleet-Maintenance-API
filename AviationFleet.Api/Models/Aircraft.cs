using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AviationFleet.Api.Models;

public class Aircraft
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string TailNumber { get; set; } = string.Empty;

    public double TotalFlightHours { get; set; }

    /// <summary>Maximum flight hours allowed since last airframe maintenance.</summary>
    public double MaintenanceThresholdHours { get; set; }

    /// <summary>Total aircraft hours when airframe maintenance was last completed.</summary>
    public double HoursAtLastMaintenance { get; set; }

    public AircraftLocationState LocationState { get; set; } = AircraftLocationState.OnGround;

    public AircraftOperationalStatus OperationalStatus { get; set; } = AircraftOperationalStatus.Ready;

    [InverseProperty(nameof(AircraftPart.Aircraft))]
    public ICollection<AircraftPart> Parts { get; set; } = new List<AircraftPart>();

    [InverseProperty(nameof(MaintenanceTicket.Aircraft))]
    public ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();
}
