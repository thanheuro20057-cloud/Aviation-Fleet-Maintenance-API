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
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
