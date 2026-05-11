using AviationFleet.Api.Dtos;

namespace AviationFleet.Api.Services;

public interface IBusinessAnalyticsService
{
    Task<BusinessDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}
