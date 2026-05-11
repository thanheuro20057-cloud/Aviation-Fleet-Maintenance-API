using AviationFleet.Api.Data;
using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;
using AviationFleet.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AviationFleetDbContext>(options =>
    options.UseInMemoryDatabase("AviationFleet_v6"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();
builder.Services.AddScoped<IMechanicRepository, MechanicRepository>();
builder.Services.AddScoped<IMaintenanceTicketRepository, MaintenanceTicketRepository>();
builder.Services.AddScoped<IFleetSettingsRepository, FleetSettingsRepository>();

builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddScoped<IMechanicService, MechanicService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IDispatcherService, DispatcherService>();
builder.Services.AddScoped<IPredictiveMaintenanceService, PredictiveMaintenanceService>();
builder.Services.AddScoped<IFlightHoursAutomationService, FlightHoursAutomationService>();
builder.Services.AddScoped<IFleetSettingsService, FleetSettingsService>();
builder.Services.AddScoped<IBusinessAnalyticsService, BusinessAnalyticsService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AviationFleetDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.FleetSettings.AnyAsync())
    {
        db.FleetSettings.Add(new FleetSettings { Id = 1, MaxMechanicActiveJobs = 3 });
        await db.SaveChangesAsync();
    }

    if (!await db.Aircraft.AnyAsync() && !await db.Mechanics.AnyAsync())
    {
        var mechanics = new[]
        {
            new Mechanic
            {
                Id = Guid.NewGuid(),
                Name = "Amelia Chen",
                NameNormalized = "amelia chen",
                Certification = CertificationType.Engine,
                IsAvailable = true,
                CurrentWorkload = 2
            },
            new Mechanic
            {
                Id = Guid.NewGuid(),
                Name = "Marco Silva",
                NameNormalized = "marco silva",
                Certification = CertificationType.Avionics,
                IsAvailable = true,
                CurrentWorkload = 1
            },
            new Mechanic
            {
                Id = Guid.NewGuid(),
                Name = "Nora Patel",
                NameNormalized = "nora patel",
                Certification = CertificationType.Hydraulics,
                IsAvailable = true,
                CurrentWorkload = 1
            },
            new Mechanic
            {
                Id = Guid.NewGuid(),
                Name = "Samir Haddad",
                NameNormalized = "samir haddad",
                Certification = CertificationType.Engine,
                IsAvailable = false,
                CurrentWorkload = 0
            }
        };

        var fleet = new[]
        {
            DemoFleetSeeder.Aircraft("C-FNVA", 1842, 500, 1368, AircraftLocationState.OnGround, AircraftOperationalStatus.InMaintenance),
            DemoFleetSeeder.Aircraft("C-FNVB", 2210, 500, 1744, AircraftLocationState.OnGround, AircraftOperationalStatus.MaintenanceRequired),
            DemoFleetSeeder.Aircraft("C-FNVC", 960, 500, 512, AircraftLocationState.InAir, AircraftOperationalStatus.Ready),
            DemoFleetSeeder.Aircraft("C-FNVD", 1415, 500, 1415, AircraftLocationState.OnGround, AircraftOperationalStatus.Ready),
            DemoFleetSeeder.Aircraft("C-FNVE", 3050, 500, 2540, AircraftLocationState.OnGround, AircraftOperationalStatus.Unavailable)
        };

        db.Mechanics.AddRange(mechanics);
        db.Aircraft.AddRange(fleet);
        db.MaintenanceTickets.AddRange(
            DemoFleetSeeder.Ticket(fleet[0], mechanics[0], "Engine vibration trend above baseline after last sector.", CertificationType.Engine, MaintenanceTicketStatus.InProgress, true, AircraftPartType.Engine, -5),
            DemoFleetSeeder.Ticket(fleet[0], mechanics[2], "Hydraulic pressure decay detected during taxi inspection.", CertificationType.Hydraulics, MaintenanceTicketStatus.InProgress, false, AircraftPartType.Hydraulics, -3),
            DemoFleetSeeder.Ticket(fleet[1], mechanics[1], "Avionics cooling fan fault. Dispatch reliability risk.", CertificationType.Avionics, MaintenanceTicketStatus.InProgress, false, AircraftPartType.Avionics, -2),
            DemoFleetSeeder.Ticket(fleet[1], null, "Engine borescope required before next rotation.", CertificationType.Engine, MaintenanceTicketStatus.Open, true, AircraftPartType.Engine, -1),
            DemoFleetSeeder.Ticket(fleet[2], null, "Landing gear inspection due soon based on utilization trend.", CertificationType.Hydraulics, MaintenanceTicketStatus.Open, true, AircraftPartType.LandingGear, -1),
            DemoFleetSeeder.Ticket(fleet[3], null, "Completed APU preventive service.", CertificationType.Engine, MaintenanceTicketStatus.Completed, true, AircraftPartType.Apu, -12));
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
