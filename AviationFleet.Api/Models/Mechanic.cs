using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AviationFleet.Api.Models;

public class Mechanic
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Normalized for uniqueness (trim + invariant lower).</summary>
    [Required]
    [MaxLength(200)]
    public string NameNormalized { get; set; } = string.Empty;

    public CertificationType Certification { get; set; }

    public bool IsAvailable { get; set; }

    public int CurrentWorkload { get; set; }

    [InverseProperty(nameof(MaintenanceTicket.Mechanic))]
    public ICollection<MaintenanceTicket> AssignedTickets { get; set; } = new List<MaintenanceTicket>();
}
