namespace TrycoreEvm.Application.Dtos;

/// <summary>Datos de entrada para crear o actualizar una actividad.</summary>
public sealed record ActivityRequest(
    string Name,
    decimal BudgetAtCompletion,
    decimal PlannedPercentComplete,
    decimal ActualPercentComplete,
    decimal ActualCost);

/// <summary>Actividad enriquecida con sus indicadores EVM calculados, tal como se expone en el API.</summary>
public sealed record ActivityResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    decimal BudgetAtCompletion,
    decimal PlannedPercentComplete,
    decimal ActualPercentComplete,
    decimal ActualCost,
    EvmIndicatorsDto Indicators);
