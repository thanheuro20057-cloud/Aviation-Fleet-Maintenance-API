using AviationFleet.Api.Models;

namespace AviationFleet.Api.Services;

public static class AircraftPartDisplay
{
    public static string Code(AircraftPartType type) =>
        type switch
        {
            AircraftPartType.Fuselage => "AIR1",
            AircraftPartType.WingStructure => "AIR2",
            AircraftPartType.Empennage => "AIR3",
            AircraftPartType.Engine => "ENG1",
            AircraftPartType.EngineCompressor => "ENG2",
            AircraftPartType.EngineCombustor => "ENG3",
            AircraftPartType.EngineTurbine => "ENG4",
            AircraftPartType.EngineFuelControl => "ENG5",
            AircraftPartType.LandingGear => "LGE1",
            AircraftPartType.NoseLandingGear => "LGE2",
            AircraftPartType.BrakeAssembly => "LGE3",
            AircraftPartType.WheelAndTire => "LGE4",
            AircraftPartType.Avionics => "AVN1",
            AircraftPartType.FlightComputer => "AVN2",
            AircraftPartType.NavigationSystem => "AVN3",
            AircraftPartType.CommunicationSystem => "AVN4",
            AircraftPartType.Transponder => "AVN5",
            AircraftPartType.Hydraulics => "HYD1",
            AircraftPartType.HydraulicReservoir => "HYD2",
            AircraftPartType.HydraulicActuator => "HYD3",
            AircraftPartType.HydraulicLines => "HYD4",
            AircraftPartType.Apu => "APU1",
            AircraftPartType.ApuStarter => "APU2",
            AircraftPartType.ApuGenerator => "APU3",
            AircraftPartType.ApuFuelSystem => "APU4",
            _ => ((int)type).ToString("D3"),
        };

    public static string Name(AircraftPartType type) =>
        type switch
        {
            AircraftPartType.Fuselage => "Fuselage / airframe",
            AircraftPartType.WingStructure => "Wing structure",
            AircraftPartType.Empennage => "Empennage / tail assembly",
            AircraftPartType.Engine => "Engine fan module",
            AircraftPartType.EngineCompressor => "Engine compressor section",
            AircraftPartType.EngineCombustor => "Engine combustor section",
            AircraftPartType.EngineTurbine => "Engine turbine section",
            AircraftPartType.EngineFuelControl => "Engine fuel control unit",
            AircraftPartType.LandingGear => "Main landing gear",
            AircraftPartType.NoseLandingGear => "Nose landing gear",
            AircraftPartType.BrakeAssembly => "Brake assembly",
            AircraftPartType.WheelAndTire => "Wheel and tire assembly",
            AircraftPartType.Avionics => "Integrated avionics bay",
            AircraftPartType.FlightComputer => "Flight control computer",
            AircraftPartType.NavigationSystem => "Navigation system",
            AircraftPartType.CommunicationSystem => "Communication radio stack",
            AircraftPartType.Transponder => "Transponder / ADS-B unit",
            AircraftPartType.Hydraulics => "Hydraulic pump",
            AircraftPartType.HydraulicReservoir => "Hydraulic reservoir",
            AircraftPartType.HydraulicActuator => "Hydraulic actuator",
            AircraftPartType.HydraulicLines => "Hydraulic lines and valves",
            AircraftPartType.Apu => "APU core",
            AircraftPartType.ApuStarter => "APU starter",
            AircraftPartType.ApuGenerator => "APU generator",
            AircraftPartType.ApuFuelSystem => "APU fuel system",
            _ => type.ToString(),
        };

    public static string Label(AircraftPartType type) => $"{Code(type)} {Name(type)}";
}
