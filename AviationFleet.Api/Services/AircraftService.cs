using AviationFleet.Api.Dtos;
using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;

namespace AviationFleet.Api.Services;

public sealed class AircraftService(
    IAircraftRepository aircraftRepository,
    IMaintenanceTicketRepository maintenanceTicketRepository,
    IMechanicRepository mechanicRepository,
    IFlightHoursAutomationService flightHoursAutomation,
    IUnitOfWork unitOfWork) : IAircraftService
{
    public async Task<IReadOnlyList<AircraftResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await aircraftRepository.GetAllAsync(cancellationToken);
        return rows.Select(Map).ToList();
    }

    public async Task<AircraftResponseDto?> CreateAsync(AircraftCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Aircraft
        {
            Id = Guid.NewGuid(),
            TailNumber = dto.TailNumber.Trim(),
            TotalFlightHours = dto.TotalFlightHours,
            MaintenanceThresholdHours = dto.MaintenanceThresholdHours,
            HoursAtLastMaintenance = dto.HoursAtLastMaintenance,
            LocationState = dto.LocationState,
            OperationalStatus = dto.OperationalStatus,
        };

        var initialPartHours = Math.Max(0, dto.TotalFlightHours - dto.HoursAtLastMaintenance);
        AircraftPartDefaults.EnsureStandardParts(entity, initialPartHours);

        aircraftRepository.Add(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<UpdateFlightHoursResultDto?> UpdateFlightHoursAsync(
        Guid id,
        UpdateAircraftFlightHoursDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await aircraftRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return null;

        var previous = entity.TotalFlightHours;
        entity.TotalFlightHours = dto.TotalFlightHours;

        var automation = await flightHoursAutomation.RunAfterTotalHoursUpdatedAsync(
            entity,
            previous,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new UpdateFlightHoursResultDto(
            Map(entity),
            automation.ThresholdExceeded,
            automation.AutoTicketCreated,
            automation.AlertMessage);
    }

    public async Task<AircraftResponseDto?> UpdateLocationAsync(
        Guid id,
        UpdateAircraftLocationDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await aircraftRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return null;

        entity.LocationState = dto.LocationState;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<AircraftResponseDto?> UpdateOperationalStatusAsync(
        Guid id,
        UpdateAircraftOperationalStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await aircraftRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return null;

        entity.OperationalStatus = dto.OperationalStatus;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<AircraftResponseDto?> ConfirmMaintenanceCompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var aircraft = await aircraftRepository.GetByIdAsync(id, cancellationToken);
        if (aircraft is null)
            return null;

        var tickets = await maintenanceTicketRepository.GetActiveTicketsForAircraftAsync(id, cancellationToken);
        foreach (var ticket in tickets)
        {
            if (ticket.MechanicId is { } mechanicId)
            {
                var mechanic = await mechanicRepository.GetByIdAsync(mechanicId, cancellationToken);
                if (mechanic is not null && mechanic.CurrentWorkload > 0)
                    mechanic.CurrentWorkload--;
            }

            ticket.Status = MaintenanceTicketStatus.Completed;
        }

        aircraft.HoursAtLastMaintenance = aircraft.TotalFlightHours;
        foreach (var part in aircraft.Parts)
            part.HoursSinceLastMaintenance = 0;

        aircraft.OperationalStatus = AircraftOperationalStatus.Ready;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(aircraft);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await aircraftRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return false;

        aircraftRepository.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AircraftResponseDto Map(Aircraft a) => new(
        a.Id,
        a.TailNumber,
        a.TotalFlightHours,
        a.MaintenanceThresholdHours,
        a.HoursAtLastMaintenance,
        a.LocationState,
        a.OperationalStatus,
        a.Parts.OrderBy(p => p.PartType)
            .Select(p => new AircraftPartResponseDto(
                p.Id,
                AircraftPartDisplay.Code(p.PartType),
                p.PartType,
                AircraftPartDisplay.Name(p.PartType),
                p.HoursSinceLastMaintenance,
                p.MaintenanceThresholdHours))
            .ToList());
}
