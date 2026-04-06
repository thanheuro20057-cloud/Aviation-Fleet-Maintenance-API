using AviationFleet.Api.Dtos;
using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;

namespace AviationFleet.Api.Services;

public sealed class MechanicService(
    IMechanicRepository mechanicRepository,
    IMaintenanceTicketRepository maintenanceTicketRepository,
    IUnitOfWork unitOfWork) : IMechanicService
{
    public async Task<IReadOnlyList<MechanicResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await mechanicRepository.GetAllAsync(cancellationToken);
        return rows.Select(Map).ToList();
    }

    public async Task<MechanicResponseDto?> CreateAsync(MechanicCreateDto dto, CancellationToken cancellationToken = default)
    {
        var normalized = dto.Name.Trim().ToLowerInvariant();
        if (await mechanicRepository.ExistsByNameNormalizedAsync(normalized, cancellationToken))
            return null;

        var entity = new Mechanic
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            NameNormalized = normalized,
            Certification = dto.Certification,
            IsAvailable = dto.IsAvailable,
            CurrentWorkload = 0,
        };

        mechanicRepository.Add(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mechanic = await mechanicRepository.GetByIdAsync(id, cancellationToken);
        if (mechanic is null)
            return false;

        var tickets = await maintenanceTicketRepository.GetTicketsAssignedToMechanicAsync(id, cancellationToken);
        foreach (var ticket in tickets)
        {
            ticket.MechanicId = null;
            if (ticket.Status == MaintenanceTicketStatus.InProgress)
                ticket.Status = MaintenanceTicketStatus.Open;
        }

        mechanicRepository.Remove(mechanic);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static MechanicResponseDto Map(Mechanic m) => new(
        m.Id,
        m.Name,
        m.Certification,
        m.IsAvailable,
        m.CurrentWorkload);
}
