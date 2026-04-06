using AviationFleet.Api.Services;

namespace AviationFleet.Tests;

public sealed class PredictiveMaintenanceRulesTests
{
    [Theory]
    [InlineData(500, 0, 464, true)]
    [InlineData(500, 0, 463, false)]
    [InlineData(500, 0, 465, true)]
    [InlineData(500, 0, 500, true)]
    [InlineData(500, 0, 600, true)]
    public void IsOnWarningRoster_UsesHoursSinceLastMaintenance(
        double threshold,
        double hoursAtLast,
        double total,
        bool expected)
    {
        var actual = PredictiveMaintenanceRules.IsOnWarningRoster(total, hoursAtLast, threshold);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HoursRemaining_UsesSinceLastMaintenance()
    {
        Assert.Equal(
            30,
            PredictiveMaintenanceRules.HoursRemainingUntilMaintenanceDue(470, 0, 500));
    }

    [Fact]
    public void WarningWindow_Is36Hours()
    {
        Assert.Equal(36, PredictiveMaintenanceRules.WarningWindowFlightHours);
    }
}
