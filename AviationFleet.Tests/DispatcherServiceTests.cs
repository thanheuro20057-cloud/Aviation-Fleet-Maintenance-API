using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;
using AviationFleet.Api.Services;
using Moq;

namespace AviationFleet.Tests;

public sealed class DispatcherServiceTests
{
    [Fact]
    public async Task DispatchAsync_WhenNoCandidate_LeavesTicketOpenAndUnassigned()
    {
        var mockRepo = new Mock<IMechanicRepository>();
        mockRepo
            .Setup(r => r.FindBestDispatchCandidateAsync(
                CertificationType.Engine,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Mechanic?)null);

        var fleet = new Mock<IFleetSettingsRepository>();
        fleet.Setup(f => f.GetMaxMechanicActiveJobsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);

        var sut = new DispatcherService(mockRepo.Object, fleet.Object);
        var ticket = new MaintenanceTicket
        {
            Id = Guid.NewGuid(),
            AircraftId = Guid.NewGuid(),
            RequiredCertification = CertificationType.Engine,
            Status = MaintenanceTicketStatus.Open,
            IssueDescription = "Test",
            DateCreated = DateTime.UtcNow,
        };

        await sut.DispatchAsync(ticket);

        Assert.Null(ticket.MechanicId);
        Assert.Equal(MaintenanceTicketStatus.Open, ticket.Status);
    }

    [Fact]
    public async Task DispatchAsync_AssignsMechanicAndIncrementsWorkload()
    {
        var mechanic = new Mechanic
        {
            Id = Guid.NewGuid(),
            Name = "Pat",
            NameNormalized = "pat",
            Certification = CertificationType.Engine,
            IsAvailable = true,
            CurrentWorkload = 2,
        };

        var mockRepo = new Mock<IMechanicRepository>();
        mockRepo
            .Setup(r => r.FindBestDispatchCandidateAsync(
                CertificationType.Engine,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mechanic);

        var fleet = new Mock<IFleetSettingsRepository>();
        fleet.Setup(f => f.GetMaxMechanicActiveJobsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);

        var sut = new DispatcherService(mockRepo.Object, fleet.Object);
        var ticket = new MaintenanceTicket
        {
            Id = Guid.NewGuid(),
            AircraftId = Guid.NewGuid(),
            RequiredCertification = CertificationType.Engine,
            Status = MaintenanceTicketStatus.Open,
            IssueDescription = "Engine fault",
            DateCreated = DateTime.UtcNow,
        };

        await sut.DispatchAsync(ticket);

        Assert.Equal(mechanic.Id, ticket.MechanicId);
        Assert.Equal(MaintenanceTicketStatus.InProgress, ticket.Status);
        Assert.Equal(3, mechanic.CurrentWorkload);
    }
}
