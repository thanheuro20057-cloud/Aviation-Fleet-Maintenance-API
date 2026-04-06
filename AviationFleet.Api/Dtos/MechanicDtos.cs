using AviationFleet.Api.Models;

namespace AviationFleet.Api.Dtos;

public sealed record MechanicCreateDto(string Name, CertificationType Certification, bool IsAvailable);

public sealed record MechanicResponseDto(Guid Id, string Name, CertificationType Certification, bool IsAvailable, int CurrentWorkload);
