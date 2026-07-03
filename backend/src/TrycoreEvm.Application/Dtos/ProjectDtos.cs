namespace TrycoreEvm.Application.Dtos;

/// <summary>Datos de entrada para crear o renombrar un proyecto.</summary>
public sealed record ProjectRequest(string Name);

/// <summary>Resumen de un proyecto sin el detalle de actividades, usado en listados.</summary>
public sealed record ProjectSummaryResponse(
    Guid Id,
    string Name,
    int ActivityCount,
    EvmIndicatorsDto ConsolidatedIndicators);

/// <summary>Proyecto completo con el detalle de todas sus actividades e indicadores.</summary>
public sealed record ProjectDetailResponse(
    Guid Id,
    string Name,
    EvmIndicatorsDto ConsolidatedIndicators,
    IReadOnlyCollection<ActivityResponse> Activities);
